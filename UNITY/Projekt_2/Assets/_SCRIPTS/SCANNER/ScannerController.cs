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
    Vector3 _pointerPosition;
    Vector3 _pointerDirection;
    bool scanning = false;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView += ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape += ExitScanner;

        GameEventsManager.instance.inputEvents.onHoldInteract += ScannerRaycast;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onEnterScanView -= ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitScanner;

        GameEventsManager.instance.inputEvents.onHoldInteract -= ScannerRaycast;
    }

    void Update()
    {
        if (scanning)
        {
            ScreenToWorldPoint();
            RotateArm();
        }
    }

    void ShowScanner(CinemachineCamera camera)
    {
        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);

        GameEventsManager.instance.inputEvents.ShowCursor(true);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);

        _camera = camera;

        _camera.Priority = 1;

        scanning = true;

        _scannerArm = Instantiate(_scannerPrefab, camera.transform);

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

        scanning = false;

        _camera.Priority = -1;

        Destroy(_scannerArm);

        GameEventsManager.instance.playerEvents.LockPlayerCamera(true);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(true);

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

        if(Physics.Raycast(ray, out RaycastHit hit, 2, _scannerRaycastLayer))
        {
            Debug.Log("Hit! " + hit.collider.gameObject);
        }
    }

}
