using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveVehicle : MonoBehaviour
{
    public static ActiveVehicle Init(Vehicle vehicle, List<Pathnode> path)
    {
        if (path == null || path.Count < 2) return null;
        Transform vehicleTransform = Instantiate(vehicle.prefab, path[0].origin, Quaternion.Euler(0, 0, 0));
        ActiveVehicle aVehicle = vehicleTransform.GetComponent<ActiveVehicle>();
        aVehicle.Create(vehicle, path);
        return aVehicle;
    }


    private Vehicle vehicle;
    public List<Vector3> path;
    private Vector3 dir;
    private bool positive = false;
    private int idx = 0;
    private float left = 0f, right = 0f;
    private float CellSize { get => BuildingSystem.Instance.grid.GetCellSize(); }

    private void Create(Vehicle vehicle, List<Pathnode> path)
    {
        this.vehicle = vehicle;
        this.path = PathnodesToVector3List(path);
        this.dir = (path[1].origin - path[0].origin).normalized;
        this.idx = 1;
        //left und right berechnen
        if (dir.x > 0 || dir.y > 0) positive = true;
        Rotate();
        transform.position = this.path[0] + dir * transform.localScale.magnitude / 2;
    }

    private void Update()
    {
        if (idx < max)
        {
            Move();
        }

    }

    public int max = 2;
    private void Move()
    {
        transform.position += dir * Time.deltaTime * vehicle.speed * 0.1f;
        NextField();
    }

    private void NextField()
    {
        if ((positive && transform.position.x >= path[idx].x && transform.position.y >= path[idx].y) ||
            (!positive && transform.position.x <= path[idx].x && transform.position.y <= path[idx].y))
        {
            if (idx + 1 == path.Count)
            {
                Destroy(gameObject);
                return;
                //TODO:
                //respawne object nach 15min IngameZeit wieder und lasse es zurückfahren
            }
            dir = (path[idx + 1] - path[idx]).normalized;
            if (dir.x > 0 || dir.y > 0) positive = true;
            else positive = false;
            Rotate();
            float surplus = Vector3.Distance(transform.position, new Vector3(path[idx].x, path[idx].y, 0f));
            idx++;
            AddSurplus(surplus);
        }
    }

    private void AddSurplus(float surplus)
    {
        Debug.Log(surplus);
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
        Vector3 nextAdditional = vehicle.type == Vehicle.VehicleType.Airplane ? new Vector3(CellSize * 0.5f, CellSize * 0.5f) : Vector3.zero;
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
                currAdditional.y = CellSize * 1 / 4 + transform.localScale.y / 2;
                nextAdditional.y = CellSize * 1 / 4 + transform.localScale.y / 2;
            }
            else if (direction.x < 0)
            {
                currAdditional.y = CellSize * 3 / 4;
                nextAdditional.y = CellSize * 3 / 4;
            }
            else if (direction.y > 0)
            {
                currAdditional.x = CellSize * 3 / 4;
                nextAdditional.x = CellSize * 3 / 4;
            }
            else
            {
                currAdditional.x = CellSize * 1 / 4;
                nextAdditional.x = CellSize * 1 / 4;
            }
            list.Add(new Vector3(path[i - 1].origin.x, path[i - 1].origin.y) + currAdditional);
        }
        list.Add(new Vector3(path[path.Count - 1].origin.x, path[path.Count - 1].origin.y) + nextAdditional);
        //Später: fahre bei der letzten Node immer noch bis zum haus oder ende der straße
        return list;
    }
}
