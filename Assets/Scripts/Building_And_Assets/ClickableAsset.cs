using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableAsset : MonoBehaviour
{
    private enum AssetType
    {
        Tower,
        Hangar
    }
    private const float doubleClickTime = 0.5f;
    private float timer = 0;
    private bool clicked = false;
    [SerializeField] private AssetType assetType = AssetType.Tower;
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (clicked && doubleClickTime - timer >= 0f)
        {
            if (assetType == AssetType.Tower)
            {
                InGameUI.Instance.OpenAirportManagerUI();
            }
            else if (assetType == AssetType.Hangar)
            {
                
                InGameUI.Instance.OpenHangarGeneralUI();
            }

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
        if (clicked) timer += Time.unscaledDeltaTime;
    }
}
