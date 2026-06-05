using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class ScanInteractor : MonoBehaviour
{
    [SerializeField] CinemachineCamera _camera;
    [SerializeField] bool _startsFishMinigame;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction += EnterScanView;
        GameEventsManager.instance.interactionEvents.onEndScannerInteraction += ExitScanView;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction -= EnterScanView;
        GameEventsManager.instance.interactionEvents.onEndScannerInteraction -= ExitScanView;
    }

    void EnterScanView(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        GameEventsManager.instance.interactionEvents.EnterScanView(_camera, gameObject);
        GameEventsManager.instance.interactionEvents.OpenFurniture(gameObject, true);

        if (_startsFishMinigame)
        {
            GameEventsManager.instance.questEvents.StartFishMinigame(true);
        }
    }

    void ExitScanView(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        GameEventsManager.instance.interactionEvents.OpenFurniture(gameObject, false);

        if (_startsFishMinigame)
        {
            GameEventsManager.instance.questEvents.StartFishMinigame(false);
        }
    }
}
