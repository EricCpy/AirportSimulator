using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class HangarCreateMenu : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private TMP_Dropdown[] airplaneTypeInputs;

    private void OnEnable()
    {
        GameManager.Instance.uiOpen = true;
        List<string> names = VehicleManager.Instance.GetAllAirplaneNames().ToList();
        foreach (var dropdown in airplaneTypeInputs) dropdown.AddOptions(names);
    }


    public void CreateHangar()
    {
        AirportManager.Instance.AddAirplanes(airplaneTypeInputs.Select(dropdown => dropdown.captionText.text).ToList());
    }

    private void OnDisable()
    {
        foreach (var dropdown in airplaneTypeInputs) dropdown.ClearOptions();
        GameManager.Instance.uiOpen = false;
    }
}
