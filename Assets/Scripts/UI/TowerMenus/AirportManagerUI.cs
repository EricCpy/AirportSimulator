using UnityEngine;
using UnityEngine.UI;

public class AirportManagerUI : MonoBehaviour
{
    [SerializeField] private Button currentSelected;
    private Color standardColor;
    [SerializeField] private Color selectedColor;
    private void Start()
    {
        standardColor = currentSelected.GetComponent<Image>().color;
        SelectButton(currentSelected);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.uiOpen)
        {
            LeaveMenu();
        }
    }

    public void LeaveMenu()
    {
        GameManager.Instance.uiOpen = false;
        gameObject.SetActive(false);
    }

    public void SelectButton(Button button)
    {
        currentSelected.GetComponent<Image>().color = standardColor;
        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }
}
