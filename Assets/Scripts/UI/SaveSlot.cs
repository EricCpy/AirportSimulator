using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slotname;
    private string id;

    public void InitalizeSaveSlot(string id)
    {
        SetName(id);
        this.id = id;
    }

    public void SetName(string slotName)
    {
        this.slotname.text = slotName;
    }

    public void LoadSaveSlot()
    {
        PlayerPrefs.SetString("LastSaveSlotID", id);
        DataManager.Instance.SetSelectedGameId(id);
        SceneManager.LoadSceneAsync(1);
        GetComponentInParent<SaveSlotMenu>().DeactivateAllSaveSlots();
    }
}
