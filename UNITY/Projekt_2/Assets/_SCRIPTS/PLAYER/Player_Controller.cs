using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{
    //PlayerInput Component
    [SerializeField] private PlayerInput playerInput;
    //Character Controller Component
    [SerializeField] private CharacterController characterController;
    //Rigidbody Component
    [SerializeField] private Rigidbody playerRb;
    //Capsule Object for testing purposes
    [SerializeField] private GameObject playerCapsule;

    //Actions defined in Action Map "Land"
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction crouchAction;
    private InputAction interactAction;
    private InputAction lookAction;

    //Properties of Player
    [Header("Player")]
    public float WalkSpeed;
    public float SprintSpeed;
    public float CrouchSpeed;

    [Header("Player Grounded")]
    public bool isGrounded;
    public float JumpStrength;
    public LayerMask GroundLayers;
    public bool AllowJump = true;

    [Header("Camera")]
    public GameObject CameraFollowTarget;
    public float TopClamp;
    public float BottomClamp;

    [Range(0.5f, 4.0f)]
    public float LookSensitivity = 1;

    private float cameraPitch;

    [SerializeField] bool isSprinting;
    [SerializeField] bool isCrouched;

    private float moveSpeed;
    private float verticalVelocity;
    private float gravityStrength = 9.81f;

    

    private void Awake()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        interactAction = playerInput.actions["Interact"];
        lookAction = playerInput.actions["Look"];
    }

    private void OnEnable()
    {
        sprintAction.performed += SetSprint;
        sprintAction.canceled += SetSprint;

        crouchAction.performed += SetCrouch;
        crouchAction.canceled +=  SetCrouch;

        jumpAction.performed += Jump;
    }

    private void OnDisable()
    {
        sprintAction.performed -= SetSprint;
        sprintAction.performed -= SetSprint;

        crouchAction.performed +=  SetCrouch;
        crouchAction.canceled -=  SetCrouch;

        jumpAction.performed -= Jump;
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
        Vector2 inputStrength = moveAction.ReadValue<Vector2>();

        gameObject.transform.Translate(Vector3.forward * moveSpeed * inputStrength.y * Time.deltaTime, Space.Self);

        gameObject.transform.Translate(Vector3.right * moveSpeed * inputStrength.x * Time.deltaTime, Space.Self);
    }

    private void Look()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        Debug.Log(lookInput);

        CameraRotation(lookInput);

        transform.Rotate(Vector3.up * lookInput.x * LookSensitivity * Time.deltaTime);
    }

    private void CameraRotation(Vector2 lookInput)
    {
        cameraPitch -= lookInput.y * LookSensitivity * Time.deltaTime;
        cameraPitch = Mathf.Clamp(cameraPitch, BottomClamp,TopClamp);

        CameraFollowTarget.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        if (isGrounded && AllowJump)
        {
            verticalVelocity += JumpStrength;
            Debug.Log(isGrounded);
        }
    }

    private void GravityAndGroundedCheck()
    {
        //Physics Check to ground
        isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 0.45f, 0), 0.5f, GroundLayers, QueryTriggerInteraction.Ignore);

        //If Player is on ground, reset vertical velocity
        if(isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        if (!isGrounded)
        {
            //Gravity
            verticalVelocity -= gravityStrength * Time.deltaTime;
        }   

        Vector3 fallVector = new Vector3(0, verticalVelocity, 0);
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
            moveSpeed = SprintSpeed;
        }
        else if (crouching)
        {
            moveSpeed = CrouchSpeed;
        }
        else
        {
            moveSpeed = WalkSpeed;
        }
    }

    private void CrouchDeform(bool toggle)
    {
        if (toggle)
        {
            playerCapsule.transform.localPosition -= new Vector3(0, 0.5f, 0);
            CameraFollowTarget.transform.localPosition -= new Vector3(0, 0.5f, 0);
            characterController.center -= new Vector3(0, 0.5f, 0);
        }
        else
        {
            playerCapsule.transform.localPosition = new Vector3(0, 1, 0);
            CameraFollowTarget.transform.localPosition = new Vector3(0, 1.6f, 0);
            characterController.center = new Vector3(0, 1, 0);
        }
    }
}
