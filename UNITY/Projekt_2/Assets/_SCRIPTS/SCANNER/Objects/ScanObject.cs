
using System.Collections;
using Febucci.TextAnimatorForUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScanObject : MonoBehaviour
{
    public ScannableObjectKind ObjectKind;
    public ScannableObjectType ObjectType;
    float _scanTimer = 3;
    bool _objectScanned;
    bool _scanningObject;
    [SerializeField] GameObject _InfoCanvasContainer;
    [SerializeField] GameObject _ButtonsCanvasContainer;
    [SerializeField] Animator _InfoCanvasAnimator;
    [SerializeField] Image _fillBarImage;
    TMP_Text _tmpText;
    TypewriterComponent _typewriter;
    MeshFilter _meshFilter;
    Renderer _renderer;
    string _descriptionText;


    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += ObjectScanOff;
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState += UpdateScannedState;

        if(ObjectKind == ScannableObjectKind.SPECIAL)
        {
            GameEventsManager.instance.questEvents.onReplaceInterpretableObjects += ReplaceGameObject;
        }
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff -= ObjectScanOff;
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState -= UpdateScannedState;

        if(ObjectKind == ScannableObjectKind.SPECIAL)
        {
            GameEventsManager.instance.questEvents.onReplaceInterpretableObjects -= ReplaceGameObject;
        }
    }

    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _renderer = GetComponent<Renderer>();
        _tmpText = _InfoCanvasContainer.GetComponentInChildren<TMP_Text>();
        _typewriter = _InfoCanvasContainer.GetComponentInChildren<TypewriterComponent>();
        _ButtonsCanvasContainer = ScannerManager.instance._interpretationButtons;
        _fillBarImage = ScannerManager.instance._scannerFillBar;
        _descriptionText = _tmpText.text;
        _tmpText.text = "";
        _fillBarImage.enabled = false;
        if(ScannerManager.instance.ScannedObjects.ContainsKey(ObjectType))
            _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];
        _InfoCanvasContainer.SetActive(false);
    }

    void Update()
    {
        if(_scanningObject && !_objectScanned)
        {
            ScanTimer();
        }
    }

    void UpdateScannedState(ScannableObjectType type)
    {
        if(type != ObjectType) return;

        _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];
        Debug.Log(_objectScanned + " / " + ScannerManager.instance.ScannedObjects[ObjectType]);
    }

    //Is called once, when Scanner Raycast hits Object
    void ObjectScanOn(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;

        _scanningObject = true;

        if (!ScannerManager.instance.ScannedObjects.ContainsKey(ObjectType))
        {
            ScannerManager.instance.ScannedObjects.Add(ObjectType, false);
            _fillBarImage.enabled = true;
        }
        else if (!_objectScanned)
        {
            _fillBarImage.enabled = true;
        }
        else if (_objectScanned)
        {
            ShowInformationCanvas(false);
        }

    }

    //Is called when Player Raycast leaves object, or is interrupted
    void ObjectScanOff(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject /*|| mode != ScannerMode.SCAN*/) return;

        Debug.Log("Entered");

        _scanningObject = false;

        _fillBarImage.enabled = false;

        if (!_objectScanned)
        {
            _scanTimer = 3;
        }

        HideInformationCanvas();
    }

    //Is called when ScanTimer is 0 for the first time
    void FirstTimeScan()
    {
        switch (ObjectKind)
        {
            case ScannableObjectKind.GENERIC:
                ShowInformationCanvas(true);
                break;
            case ScannableObjectKind.SPECIAL:
                ShowButtonCanvas(true);
                break;
            case ScannableObjectKind.QUEST:
                break;
        }
    }

    //Is called in Update when Object is being scanned
    void ScanTimer()
    {
        if(_scanTimer > 0)
        {
            _scanTimer -= 1 * Time.deltaTime;
            _fillBarImage.fillAmount = 1 - _scanTimer / 3f;
        }
        else if(_scanTimer <= 0)
        {
            ScannerManager.instance.ScannedObjects[ObjectType] = true;
            _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];
            _fillBarImage.enabled = false;
            FirstTimeScan();
        }
    }

    void ShowInformationCanvas(bool firstTimeShowing)
    {
        StopAllCoroutines();
        StartCoroutine(InfoTextAnimation(true));

        if (firstTimeShowing)
        {
            _typewriter.ShowText(_descriptionText);

            _typewriter.StartShowingText();
        }    
    }

    void HideInformationCanvas()
    {
        StopAllCoroutines();
        StartCoroutine(InfoTextAnimation(false));

        if(_tmpText.text != _descriptionText)
        {
            _tmpText.text = "";
        }
    }

    void ShowButtonCanvas(bool toggle)
    {
        GameEventsManager.instance.inputEvents.ReleaseInteract();
        GameEventsManager.instance.questEvents.ShowButtonCanvas(toggle, ObjectType);
    }

    void ReplaceGameObject(ScannableObjectType type, GameObject gameObject)
    {
        if(type != ObjectType) return;

        GameObject replacerObject = Instantiate(gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform.parent);
        replacerObject.transform.localScale = this.gameObject.transform.localScale;

        ScanObject newScanObject = replacerObject.GetComponent<ScanObject>();

        if(!ScannerManager.instance.ScannedObjects.ContainsKey(newScanObject.ObjectType))
            ScannerManager.instance.ScannedObjects.Add(newScanObject.ObjectType, true);

        else if(ScannerManager.instance.ScannedObjects.ContainsKey(newScanObject.ObjectType))
            ScannerManager.instance.ScannedObjects[newScanObject.ObjectType] = true;

        GameEventsManager.instance.interactionEvents.UpdateObjectScannedState(newScanObject.ObjectType);

        Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }

    public void SendReplaceEvent(string objectName)
    {
        GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(ObjectType, ScannerManager.instance.ScannableObjects[objectName]);
    }

    IEnumerator InfoTextAnimation(bool toggle)
    {
        switch (toggle)
        {
            case true:
                _InfoCanvasContainer.SetActive(toggle);

                _InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);
         
                break;

            case false:
                _InfoCanvasAnimator.SetBool("showText", toggle);

                yield return new WaitForSeconds(0.15f);

                _InfoCanvasContainer.SetActive(toggle);
                
                break;
        }
        
    }
}
