using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Actions : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;

    InputAction _interactAction;
    InputAction _inventoryAction;
    InputAction _escapeAction;
    InputAction _altInteractAction;
    InputAction _walkAction;
    InputAction _equipScannerAction;

    void Awake()
    {
        _interactAction = _playerInput.actions["Interact"];
        _inventoryAction = _playerInput.actions["Inventory"];
        _escapeAction = _playerInput.actions["Escape"];
        _altInteractAction = _playerInput.actions["AltInteract"];
        _walkAction = _playerInput.actions["Move"];
        _equipScannerAction = _playerInput.actions["EquipScanner"];
    }

    void OnEnable()
    {
        _interactAction.performed += Interact;
        _interactAction.canceled += ReleaseInteract;
        _inventoryAction.performed += Inventory;
        _escapeAction.performed += Escape;
        _altInteractAction.performed += AltInteract;
        _walkAction.performed += Walk;
        _equipScannerAction.performed += EquipScanner;
    }
    void OnDisable()
    {
        _interactAction.performed -= Interact;
        _interactAction.canceled -= ReleaseInteract;
        _inventoryAction.performed -= Inventory;
        _escapeAction.performed -= Escape;
        _altInteractAction.performed -= AltInteract;
        _walkAction.performed -= Walk;
        _equipScannerAction.performed -= EquipScanner;
    }

    void Update()
    {
        if (_interactAction.IsPressed())
        {
            GameEventsManager.instance.inputEvents.HoldInteract();
        }
    }

    void Interact(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        GameEventsManager.instance.inputEvents.PressedInteract();
    }

    void AltInteract(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        GameEventsManager.instance.inputEvents.PressedAltInteract();
    }

    void ReleaseInteract(InputAction.CallbackContext context)
    {
        if(!context.canceled) return;

        GameEventsManager.instance.inputEvents.ReleaseInteract();
    }

    void Inventory(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        
        GameEventsManager.instance.inputEvents.PressedInventory();
    }

    void Escape(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        GameEventsManager.instance.inputEvents.PressedEscape();
    }

    void Walk(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        Vector2 walkDirection = _walkAction.ReadValue<Vector2>();

        Debug.Log(walkDirection);

        if(walkDirection.x > 0)
        {
            GameEventsManager.instance.inputEvents.WalkRight();
        }
        if(walkDirection.x < 0)
        {
            GameEventsManager.instance.inputEvents.WalkLeft();
        }

        if(walkDirection.y > 0)
        {
            GameEventsManager.instance.inputEvents.WalkUp();
        }
        if(walkDirection.y < 0)
        {
            GameEventsManager.instance.inputEvents.WalkDown();
        }
    }

    void EquipScanner(InputAction.CallbackContext context)
    {
        if(!context.performed) return;

        GameEventsManager.instance.inputEvents.EquipScanner(_equipScannerAction.ReadValue<float>());
    }
}
