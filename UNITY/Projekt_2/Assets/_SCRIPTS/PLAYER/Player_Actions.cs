using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Actions : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;

    InputAction _interactAction;
    InputAction _inventoryAction;

    void Awake()
    {
        _interactAction = _playerInput.actions["Interact"];
        _inventoryAction = _playerInput.actions["Inventory"];
    }

    void OnEnable()
    {
        _interactAction.performed += Interact;
        _inventoryAction.performed += Inventory;
    }
    void OnDisable()
    {
        _interactAction.performed -= Interact;
        _inventoryAction.performed -= Inventory;
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
}
