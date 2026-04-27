
using System.Collections;
using Febucci.TextAnimatorForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using Microsoft.Unity.VisualStudio.Editor;

public class ScanObject : MonoBehaviour
{
    public ScannableObjectKind objectKind;
    public ScannableObjectType objectType;
    float _scanTimer = 3;
    bool _objectScanned;
    bool _scanningObject;
    [SerializeField] GameObject _canvasContainer;
    [SerializeField] Animator _canvasAnimator;
    [SerializeField]Image _fillBarImage;
    TMP_Text _tmpText;
    TypewriterComponent _typewriter;
    string _descriptionText;

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
        _tmpText = _canvasContainer.GetComponentInChildren<TMP_Text>();
        _typewriter = _canvasContainer.GetComponentInChildren<TypewriterComponent>();
        _descriptionText = _tmpText.text;
        _tmpText.text = "";
        _fillBarImage.enabled = false;
        _canvasContainer.SetActive(false);
    }

    void Update()
    {
        if(_scanningObject && !_objectScanned)
        {
            ScanTimer();
        }
    }

    void ObjectScanOn(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        _scanningObject = true;

        if (!ScannerManager.instance.ScannedObjects.ContainsKey(objectType))
        {
            ScannerManager.instance.ScannedObjects.Add(objectType, false);
            _fillBarImage.enabled = true;
        }
        else if (!_objectScanned)
        {
            _fillBarImage.enabled = true;
        }
        else if (_objectScanned)
        {
            ShowCanvas(false);
        }

    }

    void ObjectScanOff(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        _scanningObject = false;

        _fillBarImage.enabled = false;

        if (!_objectScanned)
        {
            _scanTimer = 3;
        }

        HideCanvas();
    }

    void FirstTimeScan()
    {
        switch (objectKind)
        {
            case ScannableObjectKind.GENERIC:
                ShowCanvas(true);
                break;
            case ScannableObjectKind.SPECIAL:
                break;
            case ScannableObjectKind.QUEST:
                break;
        }
    }

    void ScanTimer()
    {
        if(_scanTimer > 0)
        {
            _scanTimer -= 1 * Time.deltaTime;
            _fillBarImage.fillAmount = 1 - _scanTimer / 3f;
        }
        else if(_scanTimer <= 0)
        {
            _objectScanned = true;
            ScannerManager.instance.ScannedObjects[objectType] = true;
            _fillBarImage.enabled = false;
            FirstTimeScan();
        }
    }

    void ShowCanvas(bool firstTimeShowing)
    {
        StartCoroutine(TextAnimation(true));

        if (firstTimeShowing)
        {
            _typewriter.ShowText(_descriptionText);

            _typewriter.StartShowingText();
        }    
    }

    void HideCanvas()
    {
        StartCoroutine(TextAnimation(false));

        if(_tmpText.text != _descriptionText)
        {
            _tmpText.text = "";
        }
    }

    IEnumerator TextAnimation(bool toggle)
    {
        switch (toggle)
        {
            case true:
                _canvasContainer.SetActive(toggle);

                yield return new WaitForSeconds(0.15f);

                _canvasAnimator.SetBool("showText", toggle);
                break;

            case false:
                _canvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);

                _canvasContainer.SetActive(toggle);
                
                break;
        }
        
    }
}
