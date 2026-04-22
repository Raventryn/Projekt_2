using Unity.Cinemachine;
using UnityEngine;

public class ScanInteractor : MonoBehaviour
{
    [SerializeField] CinemachineCamera _camera;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction += EnterScanView;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction -= EnterScanView;
    }

    void EnterScanView(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        GameEventsManager.instance.interactionEvents.EnterScanView(_camera);
    }
}
