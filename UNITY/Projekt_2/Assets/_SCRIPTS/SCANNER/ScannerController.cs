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
    [SerializeField]CinemachineCamera _camera;
    GameObject _scannerArm;
    GameObject _lastScannedObject;
    Decal _xrayDecal;
    Vector3 _pointerPosition;
    Vector3 _pointerDirection;
    bool _scanning = false;
    bool _hitObject = false;
    bool _scannerLocked = true;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView += ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape += ExitScanner;
        GameEventsManager.instance.inputEvents.onReleaseInteract += ScannerOff;
        GameEventsManager.instance.inputEvents.onHoldInteract += ScannerRaycast;

        GameEventsManager.instance.interactionEvents.onChangedScannerMode += RestartScanner;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView -= ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitScanner;
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
                ScannerOff(GameEventsManager.instance.inputEvents.Context);
                if(_xrayDecal != null) _xrayDecal.enabled = false;
                //ScannerRaycast(GameEventsManager.instance.inputEvents.Context);
                break;
            case ScannerMode.XRAY:
                ScannerOff(GameEventsManager.instance.inputEvents.Context);
                
                //ScannerRaycast(GameEventsManager.instance.inputEvents.Context);
                break;
        }
    }

    void ShowScanner(CinemachineCamera camera)
    {
        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(false);

        GameEventsManager.instance.inputEvents.ShowCursor(true);

        StartCoroutine(UnlockScanner());

        _camera = camera;

        _camera.Priority = 1;

        _scanning = true;

        _scannerArm = Instantiate(_scannerPrefab, camera.transform);

        _xrayDecal = _scannerArm.GetComponentInChildren<Decal>();
        if(_xrayDecal != null) _xrayDecal.enabled = false;

        _scannerArm.transform.position += new Vector3(0.372f, -0.226f, 0.209f);
    }

    void ScreenToWorldPoint()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _depthValue));

        _pointerPosition = point;

        //Debug.Log(point);
    }

    void ExitScanner(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;

        _scanning = false;

        _scannerLocked = true;

        _camera.Priority = -1;

        Destroy(_scannerArm);

        GameEventsManager.instance.playerEvents.LockPlayerCamera(true);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);

        GameEventsManager.instance.inputEvents.ShowCursor(false);

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

        Ray ray = new Ray(_scannerArm.transform.position, _pointerDirection);

        Debug.DrawRay(_scannerArm.transform.position, _pointerDirection, Color.red);

        if(Physics.Raycast(ray, out RaycastHit hit, _scannerRange, _scannerRaycastLayer))
        {
            if(_lastScannedObject == null || _lastScannedObject != hit.collider.gameObject)
            {
                _lastScannedObject = hit.collider.gameObject;
                _hitObject = false;
            }
                
            if(!_hitObject)
            {
                GameEventsManager.instance.interactionEvents.ScanObjectOn(hit.collider.gameObject, ScannerManager.instance.ScannerMode);
                _hitObject = true;
            }

            if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = true;
        }
        else if (_hitObject)
        {
            _hitObject = false;
            ScannerOff(InputEventContext.SCANNER);
        }
    }

    void ScannerOff(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;
        _hitObject = false;
        GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject, ScannerManager.instance.ScannerMode);
        if(_xrayDecal != null && ScannerManager.instance.ScannerMode == ScannerMode.XRAY) _xrayDecal.enabled = false;
    }

    IEnumerator UnlockScanner()
    {
        yield return new WaitForSeconds(0.2f);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);
    }

}
