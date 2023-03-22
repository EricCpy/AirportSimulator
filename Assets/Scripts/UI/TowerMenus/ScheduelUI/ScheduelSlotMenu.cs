using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduelSlotMenu : MonoBehaviour
{
    [SerializeField] private GameObject scheduelSlot;
    [SerializeField] private Transform buttonBox;
    public Dictionary<ScheduelObject, ScheduelSlot> remove = new Dictionary<ScheduelObject, ScheduelSlot>();
    public void InitalizeScheduelSlots(ICollection<ScheduelObject> scheduelEntries)
    {
        foreach (var entry in scheduelEntries)
        {
            GameObject slot = Instantiate(scheduelSlot);
            slot.transform.SetParent(buttonBox);
            slot.GetComponent<ScheduelSlot>().InitalizeScheduelSlot(entry, this);
        }
    }

    public void CreateScheduelSlot(ScheduelObject scheduelEntry)
    {
        GameObject slot = Instantiate(scheduelSlot);
        slot.transform.SetParent(buttonBox);
        slot.GetComponent<ScheduelSlot>().InitalizeScheduelSlot(scheduelEntry, this);
    }
    public void RemoveScheduelEntries()
    {
        if (remove.Count > 0)
        {
            ScheduelManager.Instance.RemoveScheduelEntrys(remove.Keys);
            foreach (var slot in remove.Values)
            {
                Destroy(slot.gameObject);
            }
            remove.Clear();
        }
    }
}
