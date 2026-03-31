using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
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
    private InputAction _lookAction;

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
    public bool IsCrouchForced;

    [Header("Camera")]
    public GameObject CameraFollowTarget;
    public float TopClamp;
    public float BottomClamp;

    [Range(0.5f, 6.0f)]
    public float LookSensitivity = 1;

    private float CameraPitch;

    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouched;
    [SerializeField] bool isStanding = true;

    private float _moveSpeed;
    private float _verticalVelocity;
    private float _gravityStrength = 9.81f;

    private float _terminalVelocity = 53f;

    private float _defaultHeight;

    private bool _allowMove = true;
    private bool _allowLook = true;
    

    

    private void Awake()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
        _crouchAction = _playerInput.actions["Crouch"];
        _lookAction = _playerInput.actions["Look"];
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
    }

    private void Start()
    {
        SetMoveSpeed(false, false);
        _defaultHeight = _characterController.height;
    }

    private void Update()
    {
        Move();
        Look();
        GravityAndGroundedCheck();
        CrouchCheck();
    }

    private void Move()
    {
        if(!_allowMove) return;

        Vector2 inputStrength = _moveAction.ReadValue<Vector2>();

        /*gameObject.transform.Translate(Vector3.forward * _moveSpeed * inputStrength.y * Time.deltaTime, Space.Self);

        gameObject.transform.Translate(Vector3.right * _moveSpeed * inputStrength.x * Time.deltaTime, Space.Self);*/

        Vector3 moveDirection = new Vector3(inputStrength.x, 0.0f, inputStrength.y).normalized;

        if(inputStrength != Vector2.zero)
        {
            moveDirection = transform.right * inputStrength.x + transform.forward * inputStrength.y;
        }   

        _characterController.Move(moveDirection.normalized *(_moveSpeed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void Look()
    {
        if(!_allowLook) return;

        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        //Debug.Log(lookInput);

        CameraRotation(lookInput);

        transform.Rotate(Vector3.up * lookInput.x * LookSensitivity * Time.deltaTime);
    }

    private void CameraRotation(Vector2 lookInput)
    {
        CameraPitch -= lookInput.y * LookSensitivity * Time.deltaTime;
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
            _moveSpeed = SprintSpeed;
        }
        else if (crouching)
        {
            _moveSpeed = CrouchSpeed;
        }
        else if(!IsCrouchForced)
        {
            _moveSpeed = WalkSpeed;
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
}
