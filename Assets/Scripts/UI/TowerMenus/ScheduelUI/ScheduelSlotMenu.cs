using System.Collections.Generic;
using UnityEngine;

public class ScheduelSlotMenu : MonoBehaviour
{
    [SerializeField] private GameObject scheduelSlot;
    [SerializeField] private Transform buttonBox;
    public Dictionary<ScheduelObject, ScheduelSlot> remove = new Dictionary<ScheduelObject, ScheduelSlot>();
    private void OnEnable()
    {
        ICollection<ScheduelObject> scheduelEntries = ScheduelManager.Instance.GetAllScheduelEntries();
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

    private void OnDisable()
    {
        foreach (Transform child in buttonBox)
        {
            Destroy(child.gameObject);
        }
        remove.Clear();
    }
}
