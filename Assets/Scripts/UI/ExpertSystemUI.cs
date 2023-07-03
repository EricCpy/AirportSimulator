using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpertSystemUI : MonoBehaviour
{
    [SerializeField] private Toggle extremeEvent;
    [SerializeField] private TMP_InputField windSpeed;
    [SerializeField] private TMP_Dropdown weather;
    [SerializeField] private TMP_Text conclusion;
    [SerializeField] private List<string> weatherOrder = new List<string> { "Sun", "Rain", "Storm" };
    private void OnEnable()
    {
        StartCoroutine(UpdateValues());
    }

    private IEnumerator UpdateValues()
    {
        var delay = new WaitForSecondsRealtime(2);
        var expertSystem = ExpertSystemManager.Instance;
        while (true)
        {
            extremeEvent.isOn = expertSystem.GetExtremeEvent();
            Debug.Log(expertSystem.GetWindSpeed());
            windSpeed.text = expertSystem.GetWindSpeed() + "";
            weather.value = weatherOrder.IndexOf(expertSystem.GetWeatherAsString());
            expertSystem.AllowedToStart();
            conclusion.text = expertSystem.GetConclusion().Equals("warten") ? "Airplanes can't start!" : "Airplanes can start!";
            yield return delay;
        }
    }

    public void SetExtremeEvent()
    {
        ExpertSystemManager.Instance.SetExtremeEvent(extremeEvent.isOn);
    }

    public void SetWindSpeed()
    {
        try {
            ExpertSystemManager.Instance.SetWindSpeed(int.Parse(windSpeed.text));
        } catch(Exception e) {
            Debug.Log(e);
        }
        
    }

    public void SetWeather()
    {
        ExpertSystemManager.Instance.SetWeather(weatherOrder[weather.value]);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
