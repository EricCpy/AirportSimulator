using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangarUI : MonoBehaviour
{
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
}
