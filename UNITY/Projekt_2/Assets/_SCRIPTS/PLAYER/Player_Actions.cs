using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Actions : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;

    InputAction _interactAction;
    InputAction _inventoryAction;
    InputAction _escapeAction;

    void Awake()
    {
        _interactAction = _playerInput.actions["Interact"];
        _inventoryAction = _playerInput.actions["Inventory"];
        _escapeAction = _playerInput.actions["Escape"];
    }

    void OnEnable()
    {
        _interactAction.performed += Interact;
        _inventoryAction.performed += Inventory;
        _escapeAction.performed += Escape;
    }
    void OnDisable()
    {
        _interactAction.performed -= Interact;
        _inventoryAction.performed -= Inventory;
        _escapeAction.performed -= Escape;
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
