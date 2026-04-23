using System.Collections.Generic;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


public class ScannerController : MonoBehaviour
{
    [SerializeField] GameObject _scannerPrefab;
    [Range (0f, 10f)]
    [SerializeField] float _depthValue;
    [SerializeField] LayerMask _scannerRaycastLayer;
    [SerializeField]CinemachineCamera _camera;
    GameObject _scannerArm;
    GameObject _lastScannedObject;
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
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView -= ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitScanner;
        GameEventsManager.instance.inputEvents.onReleaseInteract -= ScannerOff;
        GameEventsManager.instance.inputEvents.onHoldInteract -= ScannerRaycast;
    }

    void Update()
    {
        if (_scanning)
        {
            ScreenToWorldPoint();
            RotateArm();
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

        _scannerArm.transform.position += new Vector3(0.372f, -0.226f, 0.209f);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);
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
        if(context != InputEventContext.SCANNER || _scannerLocked) return;

        Ray ray = new Ray(_scannerArm.transform.position, _pointerDirection);

        Debug.DrawRay(_scannerArm.transform.position, _pointerDirection, Color.red);

        if(Physics.Raycast(ray, out RaycastHit hit, 5, _scannerRaycastLayer))
        {
            if(_lastScannedObject == null || _lastScannedObject != hit.collider.gameObject)
            {
                _lastScannedObject = hit.collider.gameObject;
                _hitObject = false;
            }
                
            if(!_hitObject)
            {
                GameEventsManager.instance.interactionEvents.ScanObjectOn(hit.collider.gameObject);
                _hitObject = true;
            } 

            /*if (!ScannedObjects.ContainsKey(hit.collider.gameObject))
            {
                ScannedObjects.Add(hit.collider.gameObject, false);
                Debug.Log("Objects in dictionary: " + ScannedObjects.Count);
            }*/
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
        GameEventsManager.instance.interactionEvents.ScanObjectOff(_lastScannedObject);
    }

    IEnumerator UnlockScanner()
    {
        yield return new WaitForSeconds(0.2f);

        _scannerLocked = false;
    }

}
