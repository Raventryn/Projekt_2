using UnityEngine;
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
    private InputAction _interactAction;
    private InputAction _lookAction;

    //Properties of Player
    [Header("Player")]
    public float WalkSpeed;
    public float SprintSpeed;
    public float CrouchSpeed;

    [Header("Player Grounded")]
    public bool IsGrounded;
    public float JumpStrength;
    public LayerMask GroundLayers;
    public bool AllowJump = true;

    [Header("Camera")]
    public GameObject CameraFollowTarget;
    public float TopClamp;
    public float BottomClamp;

    [Range(0.5f, 4.0f)]
    public float LookSensitivity = 1;

    private float CameraPitch;

    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouched;

    private float _moveSpeed;
    private float _verticalVelocity;
    private float _gravityStrength = 9.81f;

    

    private void Awake()
    {
        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _sprintAction = _playerInput.actions["Sprint"];
        _crouchAction = _playerInput.actions["Crouch"];
        _interactAction = _playerInput.actions["Interact"];
        _lookAction = _playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        _sprintAction.performed += SetSprint;
        _sprintAction.canceled += SetSprint;

        _crouchAction.performed += SetCrouch;
        _crouchAction.canceled +=  SetCrouch;

        _jumpAction.performed += Jump;
    }

    private void OnDisable()
    {
        _sprintAction.performed -= SetSprint;
        _sprintAction.performed -= SetSprint;

        _crouchAction.performed +=  SetCrouch;
        _crouchAction.canceled -=  SetCrouch;

        _jumpAction.performed -= Jump;
    }

    private void Start()
    {
        SetMoveSpeed(false, false);
    }

    private void Update()
    {
        Move();
        Look();
        GravityAndGroundedCheck();
    }

    private void Move()
    {
        Vector2 inputStrength = _moveAction.ReadValue<Vector2>();

        gameObject.transform.Translate(Vector3.forward * _moveSpeed * inputStrength.y * Time.deltaTime, Space.Self);

        gameObject.transform.Translate(Vector3.right * _moveSpeed * inputStrength.x * Time.deltaTime, Space.Self);
    }

    private void Look()
    {
        Vector2 lookInput = _lookAction.ReadValue<Vector2>();

        Debug.Log(lookInput);

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
            _verticalVelocity += JumpStrength;
            Debug.Log(IsGrounded);
        }
    }

    private void GravityAndGroundedCheck()
    {
        //Physics Check to ground
        IsGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.45f, 0), 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);

        //If Player is on ground, reset vertical velocity
        if(IsGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = 0f;
        }

        if (!IsGrounded)
        {
            //Gravity
            _verticalVelocity -= _gravityStrength * Time.deltaTime;
        }   

        Vector3 fallVector = new Vector3(0, _verticalVelocity, 0);
        gameObject.transform.Translate(fallVector * Time.deltaTime, Space.Self);
    }

    

    //Sets isSprinting bool and triggers MoveSpeed change, also cancels crouch if active
    private void SetSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
            isCrouched = false;
            SetMoveSpeed(true, isCrouched);
            CrouchDeform(isCrouched);
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
        else
        {
            _moveSpeed = WalkSpeed;
        }
    }

    private void CrouchDeform(bool toggle)
    {
        if (toggle)
        {
            _playerCapsule.transform.localPosition -= new Vector3(0, 0.5f, 0);
            CameraFollowTarget.transform.localPosition -= new Vector3(0, 0.5f, 0);
            _characterController.center -= new Vector3(0, 0.5f, 0);
        }
        else
        {
            _playerCapsule.transform.localPosition = new Vector3(0, 1, 0);
            CameraFollowTarget.transform.localPosition = new Vector3(0, 1.6f, 0);
            _characterController.center = new Vector3(0, 1, 0);
        }
    }
}
