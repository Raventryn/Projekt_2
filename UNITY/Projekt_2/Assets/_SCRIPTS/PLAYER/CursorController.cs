using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    public static CursorController instance;

    [SerializeField] Player_Controller _playerController;

    public GameObject Cursor;

    public RectTransform ScreenSpaceCanvas;

    InputAction _lookAction;

    bool _IsCursorLocked;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one cursor Controller is active!");
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onShowCursor += ShowCursor;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onShowCursor -= ShowCursor;
    }

    void Start()
    {
        _lookAction = _playerController.LookAction;

        Debug.Log("Screen size:" + -Screen.width/2 + " x " + Screen.height);
    }

    void Update()
    {
        if (Cursor.activeSelf && !_IsCursorLocked)
        {
            MoveCursor();
        }
    }

    void MoveCursor()
    {
        Vector2 moveDirection = _lookAction.ReadValue<Vector2>();

        float xPosition = moveDirection.x * _playerController.LookSensitivity * Time.deltaTime;

        float yPosition = moveDirection.y * _playerController.LookSensitivity * Time.deltaTime;

        Cursor.transform.localPosition += new Vector3 (xPosition, yPosition, 0);

        float clampedScreenPositionX = Mathf.Clamp(Cursor.transform.localPosition.x, -ScreenSpaceCanvas.rect.width/2, ScreenSpaceCanvas.rect.width/2);
        float clampedScreenPositionY = Mathf.Clamp(Cursor.transform.localPosition.y, -ScreenSpaceCanvas.rect.height/2, ScreenSpaceCanvas.rect.height/2);

        Cursor.transform.localPosition = new Vector3 (clampedScreenPositionX, clampedScreenPositionY, Cursor.transform.localPosition.z);
    }

    void ShowCursor(bool toggle)
    {
        switch (toggle)
        {
            case true:
                Cursor.SetActive(true);
                break;
            case false:
                Cursor.SetActive(false);
                break;
        }
    }

    void LockCursor(bool toggle)
    {
        switch (toggle)
        {
            case true:
                _IsCursorLocked = true;
                break;
            case false:
                _IsCursorLocked = false;
                break;
        }
    }
}
