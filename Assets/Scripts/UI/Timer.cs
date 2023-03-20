using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text time;
    void Start()
    {
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer() {
        var delay = new WaitForSecondsRealtime(1);
        while (true)
        {
            time.text = ScheduelManager.Instance.airportTime.ToString();
            yield return delay;
        }
    }
}
