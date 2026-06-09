using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScannerManager : MonoBehaviour
{
    public static ScannerManager instance;

    public ScannerController Controller;

    public Dictionary<ScannableObjectType,bool> ScannedObjects;

    public ScannerMode ScannerMode = ScannerMode.SCAN;

    public List<GameObject> ScannableObjectPrefabs;

    public Dictionary<string, GameObject> ScannableObjects;

    public GameObject _interpretationButtons;

    public Image _scannerFillBar;

    public Material ObjectNotScannedMaterial;

    public Material ObjectIsScannedMaterial;

    int _scannerMode = 1;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Multiple Scanner Managers loaded");
            return;
        }
        else
        {
            instance = this;
            ScannedObjects = new Dictionary<ScannableObjectType, bool>();
        }
    }

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedAltInteract += ChangeScannerMode;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedAltInteract -= ChangeScannerMode;
    }

    void Start()
    {
        foreach(GameObject gameObject in ScannableObjectPrefabs)
        {
            ScannableObjects.Add(gameObject.name, gameObject);
        }
    }

    public void ChangeScannerMode(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER && context != InputEventContext.SCANNER_VIEW || !Controller.ScannerArm.activeSelf) return;

        switch (_scannerMode)
        {
            case 0:
                ScannerMode = ScannerMode.SCAN;
                _scannerMode = 1;
                break;
            case 1:
                ScannerMode = ScannerMode.XRAY;
                _scannerMode = 0;
                break;
        }
        
        GameEventsManager.instance.interactionEvents.ChangedScannerMode(ScannerMode);

    }

    public void ChangeScannerMode(ScannerMode mode)
    {
        switch (mode)
        {
            case ScannerMode.SCAN:
                ScannerMode = mode;
                _scannerMode = 1;
                break;
            case ScannerMode.XRAY:
                ScannerMode = mode;
                _scannerMode = 0;
                break;
        }

        GameEventsManager.instance.interactionEvents.ChangedScannerMode(ScannerMode);
        
    }
}

public enum ScannerMode
{
    SCAN,
    XRAY
}
