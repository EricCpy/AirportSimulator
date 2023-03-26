using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageSlotMenu : MonoBehaviour
{
    [SerializeField] private GameObject storageSlot;
    [SerializeField] private Transform buttonBox;
    private void OnEnable()
    {
        foreach (var airplaneCapacity in AirportManager.Instance.airplaneCapacities)
        {
            GameObject slot = Instantiate(storageSlot);
            slot.transform.SetParent(buttonBox);
            slot.GetComponent<StorageSlot>().InitalizeStorageSlot(airplaneCapacity.Key, airplaneCapacity.Value);
        }
    }

    private void OnDisable()
    {
        foreach (Transform child in buttonBox)
        {
            Destroy(child.gameObject);
        }
    }
}
