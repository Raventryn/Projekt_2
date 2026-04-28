
using UnityEngine;

public class ScannerCone : MonoBehaviour
{
    Renderer _renderer;
    bool _coneVisible = false;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract += ToggleVisibility;
        GameEventsManager.instance.inputEvents.onReleaseInteract += ToggleVisibility;

        GameEventsManager.instance.interactionEvents.onChangedScannerMode += ChangeConeColour;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract -= ToggleVisibility;
        GameEventsManager.instance.inputEvents.onReleaseInteract -= ToggleVisibility;

        GameEventsManager.instance.interactionEvents.onChangedScannerMode -= ChangeConeColour;
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

    void ChangeConeColour(ScannerMode mode)
    {
        Debug.Log(_renderer.material.color);

        switch (mode)
        {
            case ScannerMode.SCAN:
                _renderer.material.color = Color.red;
                break;
            case ScannerMode.XRAY:
                _renderer.material.color = Color.blue;
                break;
        }
    }
}
