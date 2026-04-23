using Unity.VisualScripting;
using UnityEngine;

public class ScanObject : MonoBehaviour
{
    public ScannableObjectKind objectKind;
    public ScannableObjectType objectType;
    float _scanTimer = 3;
    bool _objectScanned;
    [SerializeField] GameObject _canvasContainer;
    [SerializeField] Animator _canvasAnimator;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += ObjectScanOff;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff -= ObjectScanOff;
    }

    void Start()
    {
        _canvasContainer.SetActive(false);
    }

    void ObjectScanOn(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        if (!ScannerManager.instance.ScannedObjects.ContainsKey(objectType))
        {
            ScannerManager.instance.ScannedObjects.Add(objectType, false);

            FirstTimeScan();
        }
        else
        {
            ShowCanvas();
        }

    }

    void ObjectScanOff(GameObject gameObject)
    {
        if(gameObject != this.gameObject || !_canvasContainer.activeSelf) return;

        HideCanvas();
    }

    void FirstTimeScan()
    {
        switch (objectKind)
        {
            case ScannableObjectKind.GENERIC:
                while (!_objectScanned)
                {
                    _scanTimer -= 1 * Time.deltaTime;

                    if(_scanTimer <= 0)
                    {
                        _objectScanned = true;
                        ScannerManager.instance.ScannedObjects[objectType] = true;
                        ShowCanvas();
                        return;
                    } 
                }

                break;
            case ScannableObjectKind.SPECIAL:
                break;
            case ScannableObjectKind.QUEST:
                break;
        }
    }

    void ShowCanvas()
    {
        bool isFullScale = false;

        _canvasContainer.SetActive(true);

        _canvasContainer.transform.localScale = Vector3.zero;

        while (!isFullScale)
        {
            _canvasContainer.transform.localScale += new Vector3(0.001f, 0.001f, 0.001f);

            if(_canvasContainer.transform.localScale.magnitude >= Vector3.one.magnitude)
            {
                isFullScale = true;
            }
        }

        //Trigger text popup
    }

    void HideCanvas()
    {
        bool isZeroScale = false;

        while (!isZeroScale)
        {
            _canvasContainer.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

            if(_canvasContainer.transform.localScale.magnitude <= 0.01f)
            {
                isZeroScale = true;
                _canvasContainer.SetActive(false);
            }
        }
    }
}
