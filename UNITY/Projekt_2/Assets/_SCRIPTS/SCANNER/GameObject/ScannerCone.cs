
using UnityEngine;

public class ScannerCone : MonoBehaviour
{
    [SerializeField] Color _scanColor;
    [SerializeField] Color _xrayColor;

    Renderer _renderer;
    bool _coneVisible = false;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onChangedScannerMode += ChangeConeColour;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onChangedScannerMode -= ChangeConeColour;
    }

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
    }

    public void ToggleVisibility(bool toggle)
    {
        //if(context != InputEventContext.SCANNER && context != InputEventContext.SCANNER_VIEW) return;

        _coneVisible = toggle;
        _renderer.enabled = toggle;
    }

    void ChangeConeColour(ScannerMode mode)
    {
        switch (mode)
        {
            case ScannerMode.SCAN:
                _renderer.material.color = _scanColor;
                break;
            case ScannerMode.XRAY:
                _renderer.material.color = _xrayColor;
                break;
        }
    }
}
