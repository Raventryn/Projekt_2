using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class ScannerController : MonoBehaviour
{
    [SerializeField] GameObject _scannerPrefab;
    [Range (0f, 10f)]
    [SerializeField] float _depthValue;
    [Range (3f, 15f)]
    [SerializeField] float _scannerRange = 5;
    [SerializeField] LayerMask _scannerRaycastLayer;
    [SerializeField] LayerMask _pointerLayers;
    [SerializeField] CinemachineCamera _camera;
    [SerializeField] GameObject _scannerArm;
    GameObject _lastScannedObject;
    Decal _xrayDecal;
    Vector3 _pointerPosition;
    Vector3 _pointerDirection;
    bool _scanning = false;
    bool _hitObject = false;
    bool _scannerLocked = true;

    Animator _scannerAnimator;

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
    }

    void Update()
    {
        if(GameEventsManager.instance.inputEvents.Context != InputEventContext.SCANNER) return;

        if (_scanning)
        {
            ScreenToWorldPoint();
            RotateArm();
        }
    }

    void RestartScanner(ScannerMode mode)
    {
        switch (mode)
        {
            case ScannerMode.SCAN:
                //ScannerOff(GameEventsManager.instance.inputEvents.Context);
                _hitObject = false;
                GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
                if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;

                if(_xrayDecal != null) _xrayDecal.enabled = false;
                //ScannerRaycast(GameEventsManager.instance.inputEvents.Context);
                break;
            case ScannerMode.XRAY:
                //ScannerOff(GameEventsManager.instance.inputEvents.Context);

                _hitObject = false;
                GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
                if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;
                
                //ScannerRaycast(GameEventsManager.instance.inputEvents.Context);
                break;
        }
    }

    void ShowScanner(InputEventContext context, float value)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.SCANNER) return;

        switch (value)
        {
            case 1f:
                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);

                //_scannerArm = Instantiate(_scannerPrefab, Camera.main.transform);
                _scannerArm.SetActive(true);

                _scannerAnimator = _scannerArm.GetComponent<Animator>();

                _xrayDecal = _scannerArm.GetComponentInChildren<Decal>();
                if(_xrayDecal != null) _xrayDecal.enabled = false;

                //_scannerArm.transform.position += new Vector3(0.372f, -0.226f, 0.209f);
                break;
            case -1f:
                _scannerArm.SetActive(false);
                //Destroy(_scannerArm);
                GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
                break;
        }

        
    }

    void ToggleScanner(bool toggle)
    {
        _scanning = toggle;
    }

    void EnterScanView(CinemachineCamera camera)
    {
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(false);

        GameEventsManager.instance.inputEvents.ShowCursor(true);

        StartCoroutine(UnlockScanner());

        _camera = camera;

        _camera.Priority = 1;

        /*_scanning = true;

        _scannerArm = Instantiate(_scannerPrefab, camera.transform);

        _xrayDecal = _scannerArm.GetComponentInChildren<Decal>();
        if(_xrayDecal != null) _xrayDecal.enabled = false;

        _scannerArm.transform.position += new Vector3(0.372f, -0.226f, 0.209f);*/
    }

    void ScreenToWorldPoint()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

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
        if(context != InputEventContext.SCANNER) return;

        GameEventsManager.instance.inputEvents.ShowCursor(false);

        _scanning = false;

        _scannerLocked = true;

        _camera.Priority = -1;

        Destroy(_scannerArm);

        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
    }

    void RotateArm()
    {
        _scannerArm.transform.LookAt(_pointerPosition);
        _pointerDirection = _pointerPosition - _scannerArm.transform.position;
    }

    void ScannerRaycast(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;

        if (!_scanning)
        {
            GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
            GameEventsManager.instance.inputEvents.ShowCursor(true);
            _scanning = true;

            _scannerAnimator.SetBool("IsScanning", true);
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Debug.DrawRay(_scannerArm.transform.position, _pointerDirection, Color.red);

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
        }
        
        else if (_hitObject)
        {
            RestartScanner(ScannerManager.instance.ScannerMode);
        }

        if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY && !_xrayDecal.enabled) _xrayDecal.enabled = true;
        
    }

    void ScannerOff(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;
        _hitObject = false;

        _scannerAnimator.SetBool("IsScanning", false);

        GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
        if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;

        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);

        GameEventsManager.instance.inputEvents.ShowCursor(false);
        _scanning = false;
    }

    IEnumerator UnlockScanner()
    {
        yield return new WaitForSeconds(0.2f);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);
    }

}
