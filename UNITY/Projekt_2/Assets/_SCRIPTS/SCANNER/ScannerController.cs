using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class ScannerController : MonoBehaviour
{
    public static ScannerController instance;


    public GameObject ScannerArm;
    public GameObject XRAY_Decal;
    public bool IsScanning = false;

    [Range (0f, 10f)]
    [SerializeField] float _depthValue;
    [Range (3f, 15f)]
    [SerializeField] float _scannerRange = 5;
    [SerializeField] LayerMask _scannerRaycastLayer;
    [SerializeField] LayerMask _pointerLayers;
    [SerializeField] CinemachineCamera _camera;

    [SerializeField] Vector3[] _scanViewArmTransforms = new Vector3[3];
    Vector3[] _ArmDefaultTransforms = new Vector3[3];

    GameObject _InteractionCallerObject;

    ScannerCone _scannerCone;
    
    GameObject _lastScannedObject;
    Decal _xrayDecal;
    Vector3 _pointerPosition;
    Vector3 _pointerDirection;
    bool _hitObject = false;
    bool _scannerCoroutinePlaying;
    bool _resetArmRotation = false;
    bool _blockRC;

    public bool IsInScanView;

    Animator _scannerAnimator;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one Scanner Controller is active!");
            return;
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView += EnterScanView;
        GameEventsManager.instance.inputEvents.onPressedEscape += ExitScanView;
        GameEventsManager.instance.inputEvents.onReleaseInteract += ScannerOff;
        GameEventsManager.instance.inputEvents.onHoldInteract += ScannerRaycast;

        GameEventsManager.instance.interactionEvents.onChangedScannerMode += RestartScanner;

        GameEventsManager.instance.inputEvents.onEquipScanner += ShowScanner;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView -= EnterScanView;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitScanView;
        GameEventsManager.instance.inputEvents.onReleaseInteract -= ScannerOff;
        GameEventsManager.instance.inputEvents.onHoldInteract -= ScannerRaycast;

        GameEventsManager.instance.interactionEvents.onChangedScannerMode -= RestartScanner;

        GameEventsManager.instance.inputEvents.onEquipScanner -= ShowScanner;
    }

    void Start()
    {
        _ArmDefaultTransforms[0] = ScannerArm.transform.localPosition;
        _ArmDefaultTransforms[1] = ScannerArm.transform.localEulerAngles;
        _ArmDefaultTransforms[2] = ScannerArm.transform.localScale;

        _scannerCone = ScannerArm.GetComponentInChildren<ScannerCone>();

        ScannerArm.SetActive(false);
    }

    void Update()
    {
        if(GameEventsManager.instance.inputEvents.Context != InputEventContext.SCANNER && GameEventsManager.instance.inputEvents.Context != InputEventContext.SCANNER_VIEW) return;

        if (IsScanning)
        {
            ScreenToWorldPoint();
            RotateArm();
        }

        if (_resetArmRotation)
        {
            ResetScannerRotation();
        }
    }

    void RestartScanner(ScannerMode mode)
    {
        switch (mode)
        {
            case ScannerMode.SCAN:
                _hitObject = false;
                GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
                if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;

                if(_xrayDecal != null) _xrayDecal.enabled = false;
                break;
            case ScannerMode.XRAY:
                _hitObject = false;
                GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
                if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;
                break;
        }
    }

    void ShowScanner(InputEventContext context, float value)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.SCANNER && context != InputEventContext.SCANNER_VIEW || _scannerCoroutinePlaying) return;

        switch (value)
        {
            case 1f:
                if(context == InputEventContext.DEFAULT) GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);

                _blockRC = false;

                ScannerArm.SetActive(true);

                _scannerAnimator = ScannerArm.GetComponent<Animator>();

                _xrayDecal = ScannerArm.GetComponentInChildren<Decal>();
                if(_xrayDecal != null) _xrayDecal.enabled = false;

                StartCoroutine(UnlockScannerOnEquip(true));
                if (IsScanning)
                {
                    _scannerCone.ToggleVisibility(true);
                }
                break;
            case -1f:
                ScannerOff(GameEventsManager.instance.inputEvents.Context);
                if(context == InputEventContext.SCANNER) GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
                
                _blockRC = true;
                StartCoroutine(UnlockScannerOnEquip(false));
                
                _scannerCone.ToggleVisibility(false);
                break;
        }
    }

    void EnterScanView(CinemachineCamera camera, GameObject gameObject)
    {
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER_VIEW);

        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(false);

        if(ScannerArm.activeSelf)
        StartCoroutine(HideScannerOnTransition());

        _camera = camera;

        _camera.Priority = 1;

        _InteractionCallerObject = gameObject;

        SetScannerArmTransforms(_scanViewArmTransforms);

        IsInScanView = true;
    }

    void ScreenToWorldPoint()
    {
        //Mouse.current.position.ReadValue();
        Vector2 mousePosition = VirtualMouseCursor.instance.CursorScreenPosition;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(Screen.width / mousePosition.x, Screen.height / mousePosition.y));

        if(Physics.Raycast(ray, out RaycastHit hit, 15f, _pointerLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 hitDirection = hit.transform.position - Camera.main.transform.position;
            _depthValue = hitDirection.magnitude;
        }

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _depthValue));

        _pointerPosition = point;
    }

    void ExitScanView(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER_VIEW) return;

        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);

        GameEventsManager.instance.interactionEvents.EndScannerInteraction(_InteractionCallerObject);

        _camera.Priority = -1;

        if (ScannerArm.activeSelf)
        {
            StartCoroutine(HideScannerOnTransition());
            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);  
        }
        else
        {
            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
        }

        SetScannerArmTransforms(_ArmDefaultTransforms);
        
        IsInScanView = false;
    }

    void RotateArm()
    {
        ScannerArm.transform.LookAt(_pointerPosition);
        _pointerDirection = _pointerPosition - ScannerArm.transform.position;
    }

    void ScannerRaycast(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER && context != InputEventContext.SCANNER_VIEW || !ScannerArm.activeSelf || _blockRC) return;

        if (!IsScanning)
        {
            if(context == InputEventContext.SCANNER)
            {
                GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
                GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
            }

            GameEventsManager.instance.inputEvents.ShowCursor(true);
            IsScanning = true;
            _resetArmRotation = false;

            _scannerCone.ToggleVisibility(true);

            _scannerAnimator.SetBool("IsScanning", true);
        }

        Vector2 mousePosition = VirtualMouseCursor.instance.CursorScreenPosition;

        //Vector3 hitPosition;

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Ray debugRay = new Ray(ScannerArm.transform.position, _pointerDirection);

        Debug.DrawRay(ScannerArm.transform.position, _pointerDirection, Color.red);

        if(Physics.Raycast(ray, out RaycastHit hit, _scannerRange, _scannerRaycastLayer))
        {
            if(_lastScannedObject == null || _lastScannedObject != hit.collider.gameObject)
            {
                RestartScanner(ScannerManager.instance.ScannerMode);
                _lastScannedObject = hit.collider.gameObject;  
            }
                
            if(!_hitObject)
            {
                GameEventsManager.instance.interactionEvents.ScanObjectOn(hit.collider.gameObject, ScannerManager.instance.ScannerMode);
                _hitObject = true;
            }  

            /*if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY && _xrayDecal.enabled)
            {
                if(Physics.Raycast(debugRay, out RaycastHit hitObject, _scannerRange, _scannerRaycastLayer))
                PositionXRAYObject();
            }*/    
        }
        
        else if (_hitObject)
        {
            RestartScanner(ScannerManager.instance.ScannerMode);
        }

        if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY && !_xrayDecal.enabled)
        {
            _xrayDecal.enabled = true;
        } 
        
    }

    void ScannerOff(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER && context != InputEventContext.SCANNER_VIEW || !ScannerArm.activeSelf) return;
        _hitObject = false;

        IsScanning = false;

        _resetArmRotation = true;
        GameEventsManager.instance.inputEvents.ShowCursor(false);

        _scannerAnimator.SetBool("IsScanning", false);

        _scannerCone.ToggleVisibility(false);

        GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
        if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;

        if(context == InputEventContext.SCANNER)
        {
            GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        } 
    }

    void ResetScannerRotation()
    {
        float adjustedSpeed = 2f + 20f / Quaternion.Angle(ScannerArm.transform.localRotation, Quaternion.Euler(Vector3.zero));

        ScannerArm.transform.localRotation = Quaternion.Lerp(ScannerArm.transform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * adjustedSpeed);

        if(Quaternion.Angle(ScannerArm.transform.localRotation, Quaternion.Euler(Vector3.zero)) <= 0.1f)
        {
            ScannerArm.transform.localRotation = Quaternion.Euler(Vector3.zero);
            _resetArmRotation = false;
        } 
    }

    void PositionXRAYObject()
    {
        XRAY_Decal.transform.localPosition = new Vector3(XRAY_Decal.transform.localPosition.x, XRAY_Decal.transform.localPosition.y, _depthValue);
        //XRAY_Decal.transform.localRotation = Quaternion.Euler(_pointerDirection);
        XRAY_Decal.transform.localScale = _depthValue * Vector3.one;
    }

    void SetScannerArmTransforms(Vector3[] transforms)
    {
        ScannerArm.transform.localPosition = transforms[0];
        ScannerArm.transform.localEulerAngles = transforms[1];
        ScannerArm.transform.localScale = transforms[2];
    }

    IEnumerator HideScannerOnTransition()
    {
        ScannerOff(GameEventsManager.instance.inputEvents.Context);
        ShowScanner(InputEventContext.SCANNER_VIEW, -1);

        yield return new WaitForSeconds(0.25f);

        ShowScanner(InputEventContext.SCANNER_VIEW, 1);
        RestartScanner(ScannerManager.instance.ScannerMode);
    }

    IEnumerator UnlockScannerOnEquip(bool equipScanner)
    {
        _scannerCoroutinePlaying = true;

        switch (equipScanner)
        {
            case true:
                _scannerAnimator.SetBool("IsEquipped", true);
                
                yield return new WaitForSeconds(0.2f);
                break;
            case false:
                _scannerAnimator.SetBool("IsEquipped", false);
                
                yield return new WaitForSeconds(0.2f);

                ScannerArm.SetActive(false);
                break;
        }

        _scannerCoroutinePlaying = false;
    }

}
