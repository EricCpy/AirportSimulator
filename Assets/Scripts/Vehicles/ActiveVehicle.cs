using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveVehicle : MonoBehaviour
{
    public static ActiveVehicle Init(Vehicle vehicle, List<Pathnode> path)
    {
        if (path == null || path.Count < 2) return null;
        Debug.Log(path[0].origin);
        Transform vehicleTransform = Instantiate(vehicle.prefab, path[0].origin, Quaternion.Euler(0, 0, 0));
        ActiveVehicle aVehicle = vehicleTransform.GetComponent<ActiveVehicle>();
        aVehicle.Create(vehicle, path);
        return aVehicle;
    }

    private Vehicle vehicle;
    public List<Pathnode> path;
    private Vector3 dir;
    private bool positive = false;
    private int idx = 0;
    private void Create(Vehicle vehicle, List<Pathnode> path)
    {
        this.vehicle = vehicle;
        this.path = path;
        this.dir = (path[1].origin - path[0].origin).normalized;
        this.idx = 1;
        if (dir.x > 0 || dir.y > 0) positive = true;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Debug.Log("MOVING");
        //jetzt:
        transform.position += dir * Time.deltaTime * vehicle.speed * 0.1f;
        NextField();
        //wenn nextNode erreicht wurde
        //berechne neue dir und mache weiter, wenn es keine nächste node mehr gibt, dann despawne object
        //rotiere object nach korrekte dir


    }

    private void NextField()
    {
        if ((positive && transform.position.x >= path[idx].origin.x && transform.position.y >= path[idx].origin.y) ||
            (!positive && transform.position.x <= path[idx].origin.x && transform.position.y <= path[idx].origin.y))
        {
            if (idx + 1 == path.Count)
            {
                Destroy(gameObject);
                return;
                //TODO:
                //respawne object nach 15min IngameZeit wieder und lasse es zurückfahren
            }
            dir = (path[idx + 1].origin - path[idx].origin).normalized;
            if (dir.x > 0 || dir.y > 0) positive = true;
            else positive = false;
            float surplus = Vector3.Distance(transform.position, new Vector3(path[idx].origin.x, path[idx].origin.y, 0f));
            idx++;
            AddSurplus(surplus);
        }
    }

    private void AddSurplus(float surplus) {
        Debug.Log(surplus);
        transform.position += dir * surplus;

        NextField();
    }
}
