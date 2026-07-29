using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    public PlayerSettingsSO PlayerSettings;

    //PlayerInput Component
    [SerializeField] private PlayerInput _playerInput;
    //Character Controller Component
    [SerializeField] private CharacterController _characterController;
    //Rigidbody Component
    [SerializeField] private Rigidbody _playerRb;
    //Capsule Object for testing purposes
    [SerializeField] private GameObject _playerCapsule;

    //Actions defined in Action Map "Land"
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;
    private InputAction _crouchAction;
    public InputAction LookAction;

    //Properties of Player
    [Header("Player")]
    public float WalkSpeed;
    public float SprintSpeed;
    public float CrouchSpeed;

    [Header("Player Grounded")]
    public bool IsGrounded;
    public float JumpHeight;
    public LayerMask GroundLayers;
    public bool AllowJump = true;

    [Header("Player Crouched")]
    public bool AllowCrouch;
    public bool IsCrouchForced;

    [Header("Camera")]
    public GameObject CameraFollowTarget;
    public float TopClamp;
    public float BottomClamp;

    private float CameraPitch;

    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouched;
    [SerializeField] bool isStanding = true;

    public float MoveSpeed;
    private float _verticalVelocity;
    private float _gravityStrength = 9.81f;

    private float _terminalVelocity = 53f;

    private float _defaultHeight;

    private float _distanceWalked;

    private bool _allowMove = true;
    private bool _allowLook = true;

    private bool isPlayingStepSounds;
    
    Vector3 _cameraRestPosition;
    

    private void Awake()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
        _crouchAction = _playerInput.actions["Crouch"];
        LookAction = _playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        _sprintAction.performed += SetSprint;
        _sprintAction.canceled += SetSprint;

        _crouchAction.performed += SetCrouch;
        _crouchAction.canceled +=  SetCrouch;

        _jumpAction.performed += Jump;

        GameEventsManager.instance.playerEvents.onLockPlayerMovement += LockMovement;
        GameEventsManager.instance.playerEvents.onLockPlayerCamera += LockCamera;
        GameEventsManager.instance.playerEvents.onShowPlayerCharacter += ShowPlayer;
        //GameEventsManager.instance.inputEvents.onShowCursor += ShowCursor;
    }

    private void OnDisable()
    {
        _sprintAction.performed -= SetSprint;
        _sprintAction.performed -= SetSprint;

        _crouchAction.performed +=  SetCrouch;
        _crouchAction.canceled -=  SetCrouch;

        _jumpAction.performed -= Jump;

        GameEventsManager.instance.playerEvents.onLockPlayerMovement -= LockMovement;
        GameEventsManager.instance.playerEvents.onLockPlayerCamera -= LockCamera;
        GameEventsManager.instance.playerEvents.onShowPlayerCharacter -= ShowPlayer;
        //GameEventsManager.instance.inputEvents.onShowCursor -= ShowCursor;
    }

    private void Start()
    {
        SetLookSensitivity(PlayerSettings.LookSensitivity);
        SetMoveSpeed(false, false);
        _defaultHeight = _characterController.height;
        _cameraRestPosition = CameraFollowTarget.transform.localPosition;
        ShowCursor(false);
    }

    private void Update()
    {
        Move();
        Look();
        GravityAndGroundedCheck();
        if(AllowCrouch)
            CrouchCheck();
    }

    private void Move()
    {
        if(!_allowMove) return;

        Vector2 inputStrength = _moveAction.ReadValue<Vector2>();

        if (!isPlayingStepSounds && inputStrength.magnitude != 0)
        {
            isPlayingStepSounds = true;
            GameEventsManager.instance.soundEvents.PlayStepSounds();
        }
        else if (isPlayingStepSounds && inputStrength.magnitude == 0)
        {
            isPlayingStepSounds = false;
            GameEventsManager.instance.soundEvents.StopStepSounds();
        }

        /*gameObject.transform.Translate(Vector3.forward * _moveSpeed * inputStrength.y * Time.deltaTime, Space.Self);

        gameObject.transform.Translate(Vector3.right * _moveSpeed * inputStrength.x * Time.deltaTime, Space.Self);*/

        Vector3 moveDirection = new Vector3(inputStrength.x, 0.0f, inputStrength.y).normalized;

        if(inputStrength != Vector2.zero)
        {
            moveDirection = transform.right * inputStrength.x + transform.forward * inputStrength.y;

            _distanceWalked += MoveSpeed * Time.deltaTime;

            CameraBobOnWalk();
        }
        else
        {
            ReturnCameraToRestPosition();
        } 

        _characterController.Move(moveDirection.normalized *(MoveSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void Look()
    {
        if(!_allowLook) return;

        Vector2 lookInput = VirtualMouseCursor.instance.Delta;//LookAction.ReadValue<Vector2>(); 

        CameraRotation(lookInput);

        transform.Rotate(Vector3.up * lookInput.x * PlayerSettings.LookSensitivity * Time.deltaTime);
    }

    private void CameraRotation(Vector2 lookInput)
    {
        CameraPitch -= lookInput.y * PlayerSettings.LookSensitivity * Time.deltaTime;
        CameraPitch = Mathf.Clamp(CameraPitch, BottomClamp,TopClamp);

        CameraFollowTarget.transform.localRotation = Quaternion.Euler(CameraPitch, 0, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        if (IsGrounded && AllowJump)
        {
            _verticalVelocity = Mathf.Sqrt(JumpHeight * 2f * _gravityStrength);
            //Debug.Log(_verticalVelocity);
        }
    }

    private void GravityAndGroundedCheck()
    {
        //Physics Check to ground
        IsGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.35f, 0), 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);

        //If Player is on ground, reset vertical velocity
        if(IsGrounded && _verticalVelocity < 0.0f)
        {
            _verticalVelocity = -2f;
        }

        if (!IsGrounded && _verticalVelocity < _terminalVelocity)
        {
            //Gravity
            _verticalVelocity -= _gravityStrength * Time.deltaTime;
        }   
    }

    

    //Sets isSprinting bool and triggers MoveSpeed change, also cancels crouch if active
    private void SetSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(isCrouched) return;
            isSprinting = true;
            SetMoveSpeed(true, isCrouched);
        }
        else if (context.canceled)
        {
            isSprinting = false;
            SetMoveSpeed(false, isCrouched);
        } 
    }

    //Sets isCrouched bool and triggers MoveSpeed change, also cancels sprint if active
    private void SetCrouch(InputAction.CallbackContext context)
    {
        if(!AllowCrouch) return;

        if (context.performed)
        {
            isCrouched = true;
            isSprinting = false;
            SetMoveSpeed(isSprinting, true);
        }
        else if (context.canceled)
        {
            isCrouched = false;
            SetMoveSpeed(isSprinting, false);
        }

        CrouchDeform(isCrouched);
    }

    //Sets players move speed depending on walk, sprint or crouched
    private void SetMoveSpeed(bool sprinting, bool crouching)
    {
        if(sprinting)
        {
            MoveSpeed = SprintSpeed;
        }
        else if (crouching)
        {
            MoveSpeed = CrouchSpeed;
        }
        else if(!IsCrouchForced)
        {
            MoveSpeed = WalkSpeed;
        }
    }

    private void CrouchDeform(bool toggle)
    {
        if (toggle)
        {
            isStanding = false;
            //_playerCapsule.transform.localPosition -= new Vector3(0, 0.5f, 0);
            _characterController.height -= 1f;
            _characterController.center = new Vector3(0.0f, _characterController.height * 0.5f, 0.0f);
            CameraFollowTarget.transform.localPosition *= 0.5f;
        }
        else if(!IsCrouchForced)
        {
            isStanding = true;
            //_playerCapsule.transform.localPosition = new Vector3(0, 1, 0);
            _characterController.height = _defaultHeight;
            _characterController.center = new Vector3(0.0f, _characterController.height * 0.5f, 0.0f);
            CameraFollowTarget.transform.localPosition *= 2f;  
        }
    }

    private void CrouchCheck()
    {
        IsCrouchForced = Physics.CheckSphere(transform.position + new Vector3(0, 1.35f, 0), 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);

        if(!IsCrouchForced && !isStanding && !isCrouched)
        {
            CrouchDeform(false);
            SetMoveSpeed(false, false);
        } 
    }

    private void LockMovement(bool toggle)
    {
        if (toggle)
        {
            _allowMove = true;
        }
        else if (!toggle)
        {
            _allowMove = false;
            if (isPlayingStepSounds)
        {
            isPlayingStepSounds = false;
            GameEventsManager.instance.soundEvents.StopStepSounds();
        }
        }
    }

    private void LockCamera(bool toggle)
    {
        if (toggle)
        {
            _allowLook = true;
        }
        else if (!toggle)
        {
            _allowLook = false;
        }
    }

    private void ShowPlayer(bool toggle)
    {
        if (toggle)
        {
            _playerCapsule.GetComponent<Renderer>().enabled = true;
        }
        else if (!toggle)
        {
            _playerCapsule.GetComponent<Renderer>().enabled = false;
        }
    }

    private void ShowCursor(bool toggle)
    {
        if (toggle)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (!toggle)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void AddLookSensitivity(float value)
    {
        PlayerSettings.LookSensitivity = Mathf.Clamp(PlayerSettings.LookSensitivity += value * 0.1f, 2f, 15f);

        LookAction.ApplyParameterOverride("scaleVector2:x", PlayerSettings.LookSensitivity);
        LookAction.ApplyParameterOverride("scaleVector2:y", PlayerSettings.LookSensitivity);
    }

    public void SetLookSensitivity(float value)
    {
        PlayerSettings.LookSensitivity = Mathf.Clamp(value, 2f, 15f);

        LookAction.ApplyParameterOverride("scaleVector2:x", PlayerSettings.LookSensitivity);
        LookAction.ApplyParameterOverride("scaleVector2:y", PlayerSettings.LookSensitivity);
    }

    private void CameraBobOnWalk()
    {
        //CameraFollowTarget.transform.Translate(CameraFollowTarget.transform.up * (Mathf.Sin(Time.timeSinceLevelLoad * _moveSpeed * 5f) * 1f) * Time.deltaTime);
        float yOffset = 0;
        
        if(!isSprinting)
            yOffset = Mathf.Sin(_distanceWalked * 8f) * 0.002f;
        else
            yOffset = Mathf.Sin(_distanceWalked * 4.5f) * 0.008f;

        Vector3 targetPosition = new Vector3(CameraFollowTarget.transform.localPosition.x, CameraFollowTarget.transform.localPosition.y + yOffset, CameraFollowTarget.transform.localPosition.z);

        CameraFollowTarget.transform.localPosition = Vector3.MoveTowards(CameraFollowTarget.transform.localPosition, targetPosition, MoveSpeed);
    
        //Debug.Log(CameraFollowTarget.transform.localPosition.y + Mathf.Sin(_distanceWalked * 5f) * 0.02f);
    }

    private void ReturnCameraToRestPosition()
    {
        CameraFollowTarget.transform.localPosition = Vector3.MoveTowards(CameraFollowTarget.transform.localPosition, _cameraRestPosition, Time.deltaTime);
    }
}
