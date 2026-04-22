using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScannerController : MonoBehaviour
{
    [SerializeField] GameObject _scannerArm;
    [Range (0f, 10f)]
    [SerializeField] float _depthValue;
    Camera _camera;
    Vector3 _pointerPosition;
    bool scanning = false;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction += ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape += ExitScanner;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onScannerInteraction -= ShowScanner;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitScanner;
    }

    void Start()
    {
        _scannerArm.SetActive(false);
    }

    void Update()
    {
        if (scanning)
        {
            ScreenToWorldPoint();
            RotateArm();
        }
    }

    void ShowScanner(GameObject gameObject)
    {
        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);

        GameEventsManager.instance.inputEvents.ShowCursor(true);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.SCANNER);

        scanning = true;

        _scannerArm.SetActive(true);
    }

    void ScreenToWorldPoint()
    {
        _camera = Camera.main;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 point = _camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, _depthValue));

        _pointerPosition = point;

        Debug.Log(point);
    }

    void ExitScanner(InputEventContext context)
    {
        if(context != InputEventContext.SCANNER) return;

        scanning = false;

        _scannerArm.SetActive(false);

        GameEventsManager.instance.playerEvents.LockPlayerCamera(true);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(true);

        GameEventsManager.instance.inputEvents.ShowCursor(false);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
    }

    void RotateArm()
    {
        _scannerArm.transform.LookAt(_pointerPosition);
    }

}
