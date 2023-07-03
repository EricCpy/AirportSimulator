using UnityEngine;

public class Barrier : MonoBehaviour
{
    private bool blocked = false;
    public bool runwayDriveOn = false;
    public int runwayIndex = 0;
    [SerializeField] private bool trafficLight = false;
    private Color red = new Color(0.9f, 0.1f, 0.1f, 1f);
    private Color green = new Color(0.3f, 0.7f, 0.3f, 1f);
    [SerializeField] private SpriteRenderer[] lights;
    private void Awake()
    {
        if (lights == null) lights = GetComponentsInChildren<SpriteRenderer>();
        SetLightsColor(green);
    }
    public bool IsBlocked() { return blocked; }
    public void ToggleBlockStatus(bool blockStatus)
    {
        UnityEngine.Debug.Log("blockkkkk " + blockStatus);
        blocked = blockStatus;
        SetLightsColor(blockStatus ? red : green);
    }
    private void SetLightsColor(Color color)
    {
        foreach (var light in lights)
        {
            light.color = color;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(trafficLight) return;
        AirportManager.Instance.AirplaneLeftOrEnteredRunway(runwayDriveOn, runwayIndex);
    }

}
