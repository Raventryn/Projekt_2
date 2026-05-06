using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Actions : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;

    InputAction _interactAction;
    InputAction _inventoryAction;
    InputAction _escapeAction;
    InputAction _altInteractAction;

    void Awake()
    {
        _interactAction = _playerInput.actions["Interact"];
        _inventoryAction = _playerInput.actions["Inventory"];
        _escapeAction = _playerInput.actions["Escape"];
        _altInteractAction = _playerInput.actions["AltInteract"];
    }

    void OnEnable()
    {
        _interactAction.performed += Interact;
        _interactAction.canceled += ReleaseInteract;
        _inventoryAction.performed += Inventory;
        _escapeAction.performed += Escape;
        _altInteractAction.performed += AltInteract;
    }
    void OnDisable()
    {
        _interactAction.performed -= Interact;
        _interactAction.canceled -= ReleaseInteract;
        _inventoryAction.performed -= Inventory;
        _escapeAction.performed -= Escape;
        _altInteractAction.performed -= AltInteract;
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
}
