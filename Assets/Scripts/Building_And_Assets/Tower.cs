using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour
{
    private const float doubleClickTime = 0.5f;
    private float timer = 0;
    private bool clicked = false;
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (clicked && doubleClickTime - timer >= 0f)
        {
            InGameUI.Instance.OpenAirportManagerUI();
            clicked = false;
            timer = 0f;
        }
        else
        {
            clicked = true;
            timer = 0f;
        }
    }

    private void Update()
    {
        if (clicked) timer += Time.deltaTime;
    }
}
