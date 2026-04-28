using System.Collections;
using UnityEngine;

public class ScannerCone : MonoBehaviour
{
    Renderer _renderer;
    bool _coneVisible = false;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract += ToggleVisibility;
        GameEventsManager.instance.inputEvents.onReleaseInteract += ToggleVisibility;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract -= ToggleVisibility;
        GameEventsManager.instance.inputEvents.onReleaseInteract -= ToggleVisibility;
    }

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
    }

    void ToggleVisibility(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;

        _coneVisible = !_coneVisible;
        _renderer.enabled = _coneVisible;
    }
}
