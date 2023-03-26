using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorageSlot : MonoBehaviour
{
    [Header("InfoFields")]
    [SerializeField] private TextMeshProUGUI airplaneName;
    [SerializeField] private TextMeshProUGUI airplaneStorage;

    public void InitalizeStorageSlot(string airplaneName, int storage)
    {
        this.airplaneName.text = airplaneName;
        this.airplaneStorage.text = storage + "";
    }
}
