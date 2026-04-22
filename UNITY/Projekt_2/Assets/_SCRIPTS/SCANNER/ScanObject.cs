using Unity.VisualScripting;
using UnityEngine;

public class ScanObject : MonoBehaviour
{
    //[SerializeField] Canvas _canvas;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += ShowCanvas;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += HideCanvas;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= ShowCanvas;
        GameEventsManager.instance.interactionEvents.onScanObjectOff -= HideCanvas;
    }

    void ShowCanvas(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        //_canvas.enabled = true;

        Debug.Log("Scanned Object: " + gameObject);
    }

    void HideCanvas(GameObject gameObject)
    {
        if(gameObject != this.gameObject || !gameObject.activeSelf) return;

        Debug.Log("Hide Object: " + gameObject);
    }
}
