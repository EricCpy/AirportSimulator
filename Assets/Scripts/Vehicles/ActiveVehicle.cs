using System.Collections.Generic;
using UnityEngine;

public class ActiveVehicle : MonoBehaviour
{
    public static ActiveVehicle Init(Vehicle vehicle, List<Pathnode> path, int runwayIndex = -1)
    {
        if (path == null || path.Count < 2) return null;
        Transform vehicleTransform = Instantiate(vehicle.prefab, path[0].origin, Quaternion.Euler(0, 0, 0));
        vehicleTransform.GetComponentInChildren<SpriteRenderer>().color = vehicle.color;
        ActiveVehicle aVehicle = vehicleTransform.GetComponent<ActiveVehicle>();
        aVehicle.Create(vehicle, new List<Pathnode>(path), runwayIndex);
        return aVehicle;
    }


    private Vehicle vehicle;
    public List<Vector3> path;
    public List<Pathnode> originalPath;
    private Vector3 dir;
    private bool positive = false;
    private int idx = 0;
    private float left = 0f, right = 0f;
    private float CellSize { get => BuildingSystem.Instance.grid.GetCellSize(); }
    private int runwayIndex = -1;
    private bool lastDrive, runway;
    private bool closeDistance;
    private LayerMask mask;
    public float currentSpeed = 0f;
    private Vector2 otherObjectLastPosition;
    [SerializeField] private AnimationCurve accelerationCurve;
    private SpriteRenderer spriteRenderer;
    private void Create(Vehicle vehicle, List<Pathnode> path, int runwayIndex)
    {
        this.runwayIndex = runwayIndex;
        this.vehicle = vehicle;
        mask = LayerMask.GetMask("Default");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        InitPath(path);
    }

    public void SetRunway(bool runway)
    {
        this.runway = runway;
    }

    public void SetLastDrive(bool lastDrive)
    {
        this.lastDrive = lastDrive;
    }
    public void InitPath(List<Pathnode> path)
    {
        if (path == null) Destroy(gameObject);
        this.originalPath = path;
        this.path = PathnodesToVector3List(originalPath);
        this.dir = (originalPath[1].origin - originalPath[0].origin).normalized;
        this.idx = 1;
        Rotate();
        transform.position = this.path[0] + dir * transform.localScale.magnitude / 2;
        currentSpeed = 0;
        if (GetComponent<BoxCollider2D>()) GetComponent<BoxCollider2D>().enabled = true;
    }
    private void Update()
    {
        if (idx < path.Count)
        {
            Move();
        }
    }

    private void FixedUpdate()
    {
        if (idx >= path.Count) return;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, vehicle.sensorLength, mask);
        Debug.DrawRay(transform.position, dir * vehicle.sensorLength, Color.yellow);

        if (hit.collider != null && hit.collider.tag.Equals("CollisionAvoidanceObject"))
        {
            Barrier barrier = hit.collider.GetComponent<Barrier>();
            if (barrier != null && barrier.IsBlocked())
            {
                currentSpeed -= 30f * Time.deltaTime;
                //fahre bis auf ein Meter ran
                float sqDist = Vector3.Distance(hit.point, transform.position);
                if (currentSpeed < 1f && sqDist > 1f + spriteRenderer.bounds.size.x / 2)
                {
                    currentSpeed = 2f;
                }
            }
            else if (barrier != null)
            {
                closeDistance = false;
                currentSpeed += accelerationCurve.Evaluate(currentSpeed / vehicle.speed) * vehicle.accelerationSpeed * Time.deltaTime;
                return;
            }

            if (closeDistance)
            {
                float otherVehicleSpeed = Vector2.Distance(otherObjectLastPosition, hit.transform.position) / Time.deltaTime / 0.1f;
                if (currentSpeed > otherVehicleSpeed)
                {
                    //bremse ab auf 1 m/s weniger als das vordere Vehicle
                    currentSpeed -= 30f * Time.deltaTime;
                    currentSpeed = Mathf.Max(currentSpeed, Mathf.Max(otherVehicleSpeed - 1, 0));
                    float sqDist = Vector3.Distance(hit.point, transform.position);
                    if (currentSpeed < 1f && sqDist > 1f + spriteRenderer.bounds.size.x / 2)
                    {
                        currentSpeed = 2f;
                    }
                }
                else
                {
                    currentSpeed += accelerationCurve.Evaluate(currentSpeed / vehicle.speed) * vehicle.accelerationSpeed * Time.deltaTime;
                    currentSpeed = Mathf.Min(otherVehicleSpeed, currentSpeed);
                }
            }
            closeDistance = true;
            otherObjectLastPosition = hit.transform.position;
        }
        else
        {
            closeDistance = false;
            currentSpeed += accelerationCurve.Evaluate(currentSpeed / vehicle.speed) * vehicle.accelerationSpeed * Time.deltaTime;
        }
        currentSpeed = Mathf.Max(0, currentSpeed);
    }


    private void Move()
    {
        transform.position += dir * Time.deltaTime * currentSpeed * 0.1f;
        NextField();
    }

    private void NextField()
    {
        if (((dir.x > 0 || dir.y > 0) && (transform.position.x > path[idx].x || transform.position.y > path[idx].y)) ||
            ((dir.x < 0 || dir.y < 0) && (transform.position.x < path[idx].x || transform.position.y < path[idx].y)))
        {
            if (idx + 1 == path.Count)
            {
                if (lastDrive)
                {
                    Destroy(gameObject);
                    return;
                }
                idx++;
                if (vehicle.type == Vehicle.VehicleType.Airplane)
                {
                    AirportManager.Instance.SendVehiclesToAirplane(this, vehicle, path[path.Count - 1], runwayIndex != -1);
                    GetComponent<BoxCollider2D>().enabled = false;
                }
                else
                {
                    lastDrive = true;
                    spriteRenderer.enabled = false;
                    GetComponent<BoxCollider2D>().enabled = false;
                    Invoke("DriveBackToStart", 300f);
                }
                currentSpeed = 0;
                return;
            }
            dir = (path[idx + 1] - path[idx]).normalized;
            Rotate();
            Vector3 nextPosition = new Vector3(path[idx].x, path[idx].y, 0f);
            transform.position = nextPosition;
            float surplus = Vector3.Distance(transform.position, nextPosition);
            idx++;
            AddSurplus(surplus);
        }
    }

    private void DriveBackToStart()
    {
        spriteRenderer.enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        originalPath.Reverse();
        InitPath(originalPath);
    }

    private void AddSurplus(float surplus)
    {
        transform.position += dir * surplus;
        NextField();
    }

    private void Rotate()
    {
        if (dir.x > 0)
        {
            transform.rotation = vehicle.GetRotation(Vehicle.VehicleRotation.Right);
        }
        else if (dir.x < 0)
        {
            transform.rotation = vehicle.GetRotation(Vehicle.VehicleRotation.Left);
        }
        else if (dir.y > 0)
        {
            transform.rotation = vehicle.GetRotation(Vehicle.VehicleRotation.Up);
        }
        else
        {
            transform.rotation = vehicle.GetRotation(Vehicle.VehicleRotation.Down);
        }
    }

    private List<Vector3> PathnodesToVector3List(List<Pathnode> path)
    {
        List<Vector3> list = new List<Vector3>();
        float onequarter = CellSize * 1 / 4;
        float half = CellSize * 0.5f;
        float threequarters = CellSize * 3 / 4;
        Vector3 nextAdditional = vehicle.type == Vehicle.VehicleType.Airplane ? new Vector3(half, half) : Vector3.zero;
        for (int i = 1; i < path.Count; i++)
        {
            if (vehicle.type == Vehicle.VehicleType.Airplane)
            {
                list.Add(new Vector3(path[i - 1].origin.x, path[i - 1].origin.y) + nextAdditional);
                continue;
            }
            Vector3 currAdditional = nextAdditional;
            nextAdditional = Vector3.zero;
            Vector3 direction = (path[i].origin - path[i - 1].origin).normalized;
            if (direction.x > 0)
            {
                currAdditional.y = onequarter + transform.localScale.y / 2;
                nextAdditional.y = currAdditional.y;
            }
            else if (direction.x < 0)
            {
                currAdditional.y = threequarters;
                nextAdditional.y = currAdditional.y;
            }
            else if (direction.y > 0)
            {
                currAdditional.x = threequarters;
                nextAdditional.x = currAdditional.x ;
            }
            else
            {
                currAdditional.x = onequarter;
                nextAdditional.x = currAdditional.x;
            }
            list.Add(new Vector3(path[i - 1].origin.x, path[i - 1].origin.y) + currAdditional);
        }
        if (vehicle.type == Vehicle.VehicleType.Car) {
            if(nextAdditional.x != 0) {
                nextAdditional.y += half;
            } else {
                nextAdditional.x += half;
            }
        }    
        list.Add(new Vector3(path[path.Count - 1].origin.x, path[path.Count - 1].origin.y) + nextAdditional);

        return list;
    }

    private void OnDestroy()
    {
        if (runwayIndex != -1)
        {
            AirportManager.Instance.AirplaneLeftOrEnteredRunway(false, runwayIndex);
        }
    }

    public int GetRunwayIndex()
    {
        return runwayIndex;
    }
}
