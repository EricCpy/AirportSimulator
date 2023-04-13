using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] private Barrier upLight;
    [SerializeField] private Barrier downLight;
    [SerializeField] private Barrier leftLight;
    [SerializeField] private Barrier rightLight;
    [SerializeField] private float changeTime;
    private void Start()
    {
        StartCoroutine(ChangeLight(changeTime));
    }

    private IEnumerator ChangeLight(float changeTime)
    {
        var delay = new WaitForSeconds(changeTime);
        while (true)
        {
            if (upLight.IsBlocked())
            {
                upLight.ToggleBlockStatus(false);
                downLight.ToggleBlockStatus(false);
                leftLight.ToggleBlockStatus(true);
                rightLight.ToggleBlockStatus(true);
            }
            else
            {
                leftLight.ToggleBlockStatus(false);
                rightLight.ToggleBlockStatus(false);
                upLight.ToggleBlockStatus(true);
                downLight.ToggleBlockStatus(true);
            }
            yield return delay;
        }
    }
}
