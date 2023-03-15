using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneSlotMenu : MonoBehaviour
{
    [SerializeField] private GameObject airplaneSlot;
    [SerializeField] private Transform buttonBox;
    public Dictionary<Vehicle, AirplaneSlot> remove = new Dictionary<Vehicle, AirplaneSlot>();
    public void InitalizeAirplaneSlots(ICollection<Vehicle> airplanes)
    {
        foreach (var airplane in airplanes)
        {
            GameObject slot = Instantiate(airplaneSlot);
            slot.transform.SetParent(buttonBox);
            slot.GetComponent<AirplaneSlot>().InitalizeAirplaneSlot(airplane, this);
        }
    }

    public void RemoveAirplanes()
    {
        if (remove.Count > 0)
        {
            VehicleManager.Instance.RemoveAirplanes(remove.Keys);
            foreach(var slot in remove.Values) {
                Destroy(slot.gameObject);
            }
            remove.Clear();
        }
    }

}
