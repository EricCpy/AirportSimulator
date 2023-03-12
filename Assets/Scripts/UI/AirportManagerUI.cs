using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AirportManagerUI : MonoBehaviour
{
    public GameObject airportMenu;
    private const float doubleClickTime = 0.5f;
    private float timer = 0;
    private bool clicked = false;
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
        if (clicked) timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.uiOpen)
        {
            LeaveMenu();
        }
    }

    public void LeaveMenu()
    {
        airportMenu.SetActive(false);
        GameManager.Instance.uiOpen = false;
    }

    public void OpenMenu()
    {
        airportMenu.SetActive(true);
        GameManager.Instance.uiOpen = true;
    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (clicked && doubleClickTime - timer >= 0f)
        {
            OpenMenu();
            clicked = false;
            timer = 0f;
        }
        else
        {
            clicked = true;
            timer = 0f;
        }
    }

    public void SelectButton(Button button)
    {
        currentSelected.GetComponent<Image>().color = standardColor;
        currentSelected = button;
        currentSelected.GetComponent<Image>().color = selectedColor;
    }
}
