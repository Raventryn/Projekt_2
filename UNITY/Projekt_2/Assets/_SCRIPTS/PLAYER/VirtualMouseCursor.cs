using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

//https://www.youtube.com/watch?v=Y3WNwl1ObC8
public class VirtualMouseCursor : MonoBehaviour
{
    public static VirtualMouseCursor instance;

    public Vector3 CursorScreenPosition;

    public bool IsCursorVisible;

    [SerializeField] PlayerSettingsSO _playerSettings;
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] RectTransform _cursorTransform;
    [SerializeField] RectTransform _canvasTransform;
    [SerializeField]Camera _canvasCamera;
    [SerializeField] float _pointerSpeedMultiplier = 6f;
    Canvas _canvas;
    
    private Mouse virtualMouse;
    bool _previousMouseState;

    void Awake()
    {
        if(instance != null)
        {
            Debug.Log("More than one virtual mouse active!");
            return;
        }
        else
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        _canvas = _canvasTransform.GetComponent<Canvas>();

        //Add virtual Mouse to Input System
        if(virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        //Pair device to user of PlayerInput Component with Event System & Virtual Mouse
        InputUser.PerformPairingWithDevice(virtualMouse, _playerInput.user);

        if(_cursorTransform != null)
        {
            Vector2 position = _cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;

        GameEventsManager.instance.inputEvents.onShowCursor += ShowCursor;

        AnchorCursor(new Vector2(Screen.width/2, Screen.height/2));
    }

    private void OnDisable()
    {
        /*if(virtualMouse != null && virtualMouse.added)
        {
            
        }*/
        //_playerInput.user.UnpairDevice(virtualMouse);
        InputSystem.RemoveDevice(virtualMouse);
        
        InputSystem.onAfterUpdate -= UpdateMotion;

        GameEventsManager.instance.inputEvents.onShowCursor -= ShowCursor;
    }

    void UpdateMotion()
    {
        if(virtualMouse == null || !IsCursorVisible) return;

        Vector2 deltaValue = Mouse.current.delta.ReadValue();//Gamepad.current.leftStick.ReadValue();
        deltaValue *= _playerSettings.LookSensitivity * _pointerSpeedMultiplier * Time.unscaledDeltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, 0f + _cursorTransform.rect.width, Screen.width - _cursorTransform.rect.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0f + _cursorTransform.rect.height, Screen.height - _cursorTransform.rect.height);

        //Debug.Log(newPosition);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        CursorScreenPosition = virtualMouse.position.ReadValue();

        /*Vector2 moveDirection = Mouse.current.delta.ReadValue();

        float xPosition = moveDirection.x * _playerController.LookSensitivity * Time.deltaTime;

        float yPosition = moveDirection.y * _playerController.LookSensitivity * Time.deltaTime;

        _cursorTransform.localPosition += new Vector3 (xPosition, yPosition, 0);

        float clampedScreenPositionX = Mathf.Clamp(_cursorTransform.localPosition.x, 0f, Screen.width);
        float clampedScreenPositionY = Mathf.Clamp(_cursorTransform.localPosition.y, 0f, Screen.height);

        _cursorTransform.localPosition = new Vector3 (clampedScreenPositionX, clampedScreenPositionY, _cursorTransform.localPosition.z);*/

        bool leftButtonisPressed = Mouse.current.leftButton.IsPressed();//Gamepad.current.aButton.IsPressed();
        if(_previousMouseState != leftButtonisPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, leftButtonisPressed);
            InputState.Change(virtualMouse, mouseState);
            _previousMouseState = leftButtonisPressed;
        } 
        
        AnchorCursor(newPosition);
    }

    void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, position, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvasCamera, out anchoredPosition);

        _cursorTransform.anchoredPosition = anchoredPosition;
    }

    void ShowCursor(bool toggle)
    {
        switch (toggle)
        {
            case true:
                AnchorCursor(new Vector2(Screen.width/2, Screen.height/2));
                IsCursorVisible = true;
                _cursorTransform.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case false:
                IsCursorVisible = false;
                _cursorTransform.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                AnchorCursor(new Vector2(Screen.width/2, Screen.height/2));
                break;
        }
    }
}