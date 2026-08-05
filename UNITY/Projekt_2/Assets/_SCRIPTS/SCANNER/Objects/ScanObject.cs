
using System.Collections;
using Febucci.TextAnimatorForUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScanObject : MonoBehaviour
{
    ShowCanvas _showCanvas;
    InterpretObject _interpretObject;
    TriggerMinigame _triggerMinigame;

    public string Name;
    public ScannableObjectKind ObjectKind;
    public ScannableObjectType ObjectType;
    public GameObject ScanObjectGO;
    public Collider Collider;
    float _scanTimer = 1;
    bool _objectScanned;
    bool _scanningObject;
    [SerializeField] GameObject _InfoCanvasContainer;
    [SerializeField] Animator _InfoCanvasAnimator;
    public ParticleSystem _GlitchParticles;
    [SerializeField] ScanObject[] _InterpretationOptions = new ScanObject[3];
    GameObject _ButtonsCanvasContainer;
    Image _fillBarImage;
    TMP_Text _tmpText;
    TypewriterComponent _typewriter;
    MeshFilter _meshFilter;
    Renderer _renderer;
    public Material DefaultMaterial;
    float _defaultScaleValue;
    float _pulseScaleValue;
    bool _isPulseObject;
    bool _isObjectPulseSize;
    bool _hasParticles;
    string _descriptionText;


    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn += ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff += ObjectScanOff;
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState += UpdateScannedState;
        GameEventsManager.instance.questEvents.onHideScanGlitch += HideScanGlitch;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScanObjectOn -= ObjectScanOn;
        GameEventsManager.instance.interactionEvents.onScanObjectOff -= ObjectScanOff;
        GameEventsManager.instance.interactionEvents.onUpdateObjectScannedState -= UpdateScannedState;
        GameEventsManager.instance.questEvents.onHideScanGlitch -= HideScanGlitch;
    }

    void Start()
    {
        _meshFilter = ScanObjectGO.GetComponent<MeshFilter>();
        _renderer = ScanObjectGO.GetComponent<Renderer>();
        _tmpText = _InfoCanvasContainer.GetComponentInChildren<TMP_Text>();
        _typewriter = _InfoCanvasContainer.GetComponentInChildren<TypewriterComponent>();
        _ButtonsCanvasContainer = ScannerManager.instance._interpretationButtons;
        _fillBarImage = ScannerManager.instance._scannerFillBar;
        _descriptionText = _tmpText.text;
        _tmpText.text = "";
        _fillBarImage.enabled = false;
        DefaultMaterial = _renderer.material;
        _defaultScaleValue = gameObject.transform.localScale.x;
        _pulseScaleValue = _defaultScaleValue * 1.1f;
        Collider = GetComponent<Collider>();
        if(ScannerManager.instance.ScannedObjects.ContainsKey(ObjectType))
            _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];

        if (_objectScanned || ObjectKind == ScannableObjectKind.GENERIC)
        {
            HideScanGlitch(ObjectType);
        } 
        
        _InfoCanvasContainer.SetActive(false);

        if(!_objectScanned && ObjectKind != ScannableObjectKind.GENERIC)
            ChangeMaterial(ScannerManager.instance.ObjectNotScannedMaterial);

        InstantiateClasses();
    }

    void Update()
    {
        if(_scanningObject && !_objectScanned && ScannerController.instance.IsScanning)
        {
            ScanTimer();
        }
        else if(_scanningObject && !_objectScanned && !ScannerController.instance.IsScanning)
        {
            ObjectScanOff(this.gameObject, ScannerManager.instance.ScannerMode);
        }

        if (_isPulseObject)
        {
            PulseObjectOnScan();
        }
    }

    void InstantiateClasses()
    {
        switch (ObjectKind)
        {
            case ScannableObjectKind.INTERPRETABLE:
                _interpretObject = gameObject.AddComponent<InterpretObject>();
                _interpretObject.ObjectType = ObjectType;
                _interpretObject.InterpretationOptions = _InterpretationOptions;
                break;
            case ScannableObjectKind.MINIGAME:
                _triggerMinigame = gameObject.AddComponent<TriggerMinigame>();
                break;
        }

        _showCanvas = gameObject.AddComponent<ShowCanvas>();
        
        _showCanvas.TmpText = _tmpText;
        _showCanvas.Typewriter = _typewriter;
        _showCanvas.InfoCanvasContainer = _InfoCanvasContainer;
        _showCanvas.InfoCanvasAnimator = _InfoCanvasAnimator;
        _showCanvas.DescriptionText = _descriptionText;
    }

    void UpdateScannedState(ScannableObjectType type)
    {
        if(type != ObjectType) return;

        _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];

        if(_objectScanned == true)
        {
            ChangeMaterial(DefaultMaterial);
            if(_GlitchParticles != null)
                _GlitchParticles.gameObject.SetActive(false);
        }
        else
        {
            ChangeMaterial(ScannerManager.instance.ObjectNotScannedMaterial);
        }

        //Debug.Log(_objectScanned + " / " + ScannerManager.instance.ScannedObjects[ObjectType]);
    }

    //Is called once, when Scanner Raycast hits Object
    void ObjectScanOn(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject || mode != ScannerMode.SCAN) return;

        _scanningObject = true;

        _isPulseObject = true;

        ChangeMaterial(ScannerManager.instance.ObjectIsScannedMaterial);

        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.SCAN_START, false);

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
            _showCanvas.ShowInformationCanvas(false);
        }

    }

    //Is called when Player Raycast leaves object, or is interrupted
    void ObjectScanOff(GameObject gameObject, ScannerMode mode)
    {
        if(gameObject != this.gameObject /*|| mode != ScannerMode.SCAN*/) return;

        //Debug.Log("Entered");

        _scanningObject = false;

        _fillBarImage.enabled = false;

        if (!_objectScanned)
        {
            _scanTimer = 1;
            _fillBarImage.fillAmount = 1 - _scanTimer / 1f;
        }

        if (_objectScanned || ObjectKind == ScannableObjectKind.GENERIC)
        {
            ChangeMaterial(DefaultMaterial);
        }
        else
        {
            ChangeMaterial(ScannerManager.instance.ObjectNotScannedMaterial);
        }

        _showCanvas.HideInformationCanvas();
    }

    //Is called when ScanTimer is 0 for the first time
    void FirstTimeScan()
    {
        GameEventsManager.instance.questEvents.ObjectScanned();

        switch (ObjectKind)
        {
            case ScannableObjectKind.GENERIC:
                ScannerManager.instance.ScannedObjects[ObjectType] = true;
                _objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];
                _showCanvas.ShowInformationCanvas(true);
                ExperienceManager.instance.AddMoney(Random.Range(3, 10));
                break;
            case ScannableObjectKind.INTERPRETABLE:
                //ScannerManager.instance.ScannedObjects[ObjectType] = true;
                //_objectScanned = ScannerManager.instance.ScannedObjects[ObjectType];
                _interpretObject.ShowButtonCanvas(true);
                //Add Money after interpreting
                break;
            case ScannableObjectKind.MINIGAME:
                _triggerMinigame.OpenMinigame(true, ObjectType);
                GameEventsManager.instance.inputEvents.ReleaseInteract();
                GameEventsManager.instance.inputEvents.EquipScanner(-1);
                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.CALIBRATING);
                //Add Money after Minigame
                break;
        }
    }

    //Is called in Update when Object is being scanned
    void ScanTimer()
    {
        if(_scanTimer > 0)
        {
            _scanTimer -= 1 * Time.deltaTime;
            _fillBarImage.fillAmount = 1 - _scanTimer / 1f;
        }
        else if(_scanTimer <= 0)
        {
            _fillBarImage.enabled = false;
            FirstTimeScan();
        }
    }

    public void ChangeMaterial(Material material)
    {
        if(_renderer != null)
        _renderer.material = material;
    }

    void PulseObjectOnScan()
    {
        if(transform.localScale.x < _pulseScaleValue && !_isObjectPulseSize)
        {
            transform.localScale += Vector3.one * 2 * Time.deltaTime;

            if(transform.localScale.x >= _pulseScaleValue)
            {
                _isObjectPulseSize = true;
            }
        }
        else if(transform.localScale.x > _defaultScaleValue && _isObjectPulseSize)
        {
            transform.localScale -= Vector3.one * 2 * Time.deltaTime;

            if(transform.localScale.x <= _defaultScaleValue)
            {
                transform.localScale = Vector3.one * _defaultScaleValue;
                
                _isPulseObject = false;
                _isObjectPulseSize = false;
            }
        }
        
    }

    public void HideScanGlitch(ScannableObjectType type)
    {
        if(type != ObjectType) return;
        if(_GlitchParticles == null)
        {
           return; 
        } 
        _GlitchParticles.Stop();
        _GlitchParticles.gameObject.SetActive(false);
        
    }

    public void ToggleCollider(bool toggle)
    {
        Collider.enabled = toggle;
    }
}
