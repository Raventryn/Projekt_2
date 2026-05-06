using UnityEngine;
using UnityEngine.PlayerLoop;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] GameObject _UIContentParent;

    GameObject _currentItem;
    GameObject _previousItem;
    GameObject _nextItem;

    InputEventContext _previousContext;

    int _inventoryLength;
    int _currentlyDisplayedItem = 0;
    bool _isUIenabled = false;

    void OnEnable()
    {
       GameEventsManager.instance.inputEvents.onPressedInventory += ToggleUI; 
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInventory -= ToggleUI; 
    }

    void ToggleUI(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.INVENTORY) return;
        Debug.Log("Entered");
        _isUIenabled = !_isUIenabled;

        _UIContentParent.SetActive(_isUIenabled);

        if (_UIContentParent.activeSelf)
        {
            _previousContext = context;

            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.INVENTORY);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

            UpdateInventory();
            ShowInventoryObjects();
        }
        else
        {
            DestroyInventoryObjects();

            GameEventsManager.instance.inputEvents.ChangeInputContext(_previousContext);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        }      
    }

    void UpdateInventory()
    {
        _inventoryLength = _inventorySystem.inventory.Count;
    }

    void ShowInventoryObjects()
    {
        GameObject currentItem;
        GameObject previousItem;
        GameObject nextItem;

        if(_currentlyDisplayedItem == 0)
        {
            currentItem = _inventorySystem.inventory[_currentlyDisplayedItem].Data.ItemPrefab;
            previousItem = _inventorySystem.inventory[_inventoryLength - 1].Data.ItemPrefab;
            nextItem = _inventorySystem.inventory[_currentlyDisplayedItem + 1].Data.ItemPrefab;
        }
        else if(_currentlyDisplayedItem == _inventoryLength - 1)
        {
            currentItem = _inventorySystem.inventory[_currentlyDisplayedItem].Data.ItemPrefab;
            previousItem = _inventorySystem.inventory[_currentlyDisplayedItem - 1].Data.ItemPrefab;
            nextItem = _inventorySystem.inventory[0].Data.ItemPrefab;
        }
        else
        {
            currentItem = _inventorySystem.inventory[_currentlyDisplayedItem].Data.ItemPrefab;
            previousItem = _inventorySystem.inventory[_currentlyDisplayedItem - 1].Data.ItemPrefab;
            nextItem = _inventorySystem.inventory[_currentlyDisplayedItem + 1].Data.ItemPrefab;
        }
        
        _currentItem = Instantiate(currentItem,_UIContentParent.transform);
        _previousItem = Instantiate(previousItem, ItemPosition("left"), Quaternion.Euler(Vector3.zero), _UIContentParent.transform);
        _nextItem = Instantiate(nextItem, ItemPosition("right"), Quaternion.Euler(Vector3.zero), _UIContentParent.transform);

        _currentItem.layer = 5;
        _previousItem.layer = 5;
        _nextItem.layer = 5;
    }

    Vector3 ItemPosition(string position)
    {
        Vector3 newItemPosition = _UIContentParent.transform.transform.position;

        switch (position)
        {
            case "right":
                newItemPosition += new Vector3(3.25f, 0, 2); 
                break;
            case "left":
                newItemPosition -= new Vector3(3.25f, 0, -2); 
                break;
        }

        return newItemPosition;
    }

    void DestroyInventoryObjects()
    {
        Destroy(_currentItem);
        Destroy(_previousItem);
        Destroy(_nextItem);
    }
}
