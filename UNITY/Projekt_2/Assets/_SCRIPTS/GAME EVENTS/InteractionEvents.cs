using System;
using Unity.Cinemachine;
using UnityEngine;

public class InteractionEvents
{
    public event Action<GameObject> onInteraction;
    
    public void Interaction(GameObject gameObject)
    {
        onInteraction?.Invoke(gameObject);
    }

    public event Action<GameObject> onPickUpInteraction;

    public void PickUpInteraction(GameObject gameObject)
    {
        onPickUpInteraction?.Invoke(gameObject);
    }

    public event Action<GameObject> onDialogueInteraction;

    public void DialogueInteraction(GameObject gameObject)
    {
        onDialogueInteraction?.Invoke(gameObject);
    }

    public event Action<GameObject> onScannerInteraction;

    public void ScannerInteraction(GameObject gameObject)
    {
        onScannerInteraction?.Invoke(gameObject);
    }

    public event Action<CinemachineCamera> onEnterScanView;

    public void EnterScanView(CinemachineCamera camera)
    {
        onEnterScanView?.Invoke(camera);
    }
}
