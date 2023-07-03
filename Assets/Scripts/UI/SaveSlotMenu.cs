using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SaveSlotMenu : MonoBehaviour
{
    List<Button> saveSlots = new List<Button>();
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject saveSlot;
    [SerializeField] private Transform buttonBox;
    public void InitalizeAllSaveSlots(HashSet<string> set)
    {
        foreach (string s in set)
        {
            GameObject slot = Instantiate(saveSlot);
            slot.transform.SetParent(buttonBox);
            slot.GetComponent<SaveSlot>().InitalizeSaveSlot(s);
            saveSlots.Add(slot.GetComponent<Button>());
        }
    }

    public void DeactivateAllSaveSlots()
    {
        foreach (var saveSlot in saveSlots)
        {
            saveSlot.interactable = false;
        }
        backButton.interactable = false;
    }
}
