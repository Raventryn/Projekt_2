using System.Collections;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] GameObject _UIContentParent;
    [SerializeField] float _scrollSpeed = 2;

    GameObject _currentItem;
    GameObject _previousItem;
    GameObject _nextItem;
    GameObject _newItem;

    InputEventContext _previousContext;

    Vector3[] _slotPositions;

    int _inventoryLength;
    int _currentlyDisplayedItem = 0;
    bool _isUIenabled = false;

    bool _moveItemsLeft;
    bool _moveItemsRight;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedInventory += ToggleUI;
        GameEventsManager.instance.inputEvents.onWalkLeft += ScrollPreviousItem;
        GameEventsManager.instance.inputEvents.onWalkRight += ScrollNextItem;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInventory -= ToggleUI; 
        GameEventsManager.instance.inputEvents.onWalkLeft -= ScrollPreviousItem;
        GameEventsManager.instance.inputEvents.onWalkRight -= ScrollNextItem;
    }

    void Start()
    {
        SetSlotPositions();
    }

    void Update()
    {
        if (_moveItemsLeft)
        {
            ScrollAnimation("next");
        }
        else if (_moveItemsRight)
        {
            ScrollAnimation("previous");
        }
    }

    void ToggleUI(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight) return;
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
            InstantiateInventoryItems(GetInventoryItemIndex(_currentlyDisplayedItem));

            Debug.Log("Current: " + _currentItem.name);
        }
        else
        {
            DestroyInventoryItems();

            GameEventsManager.instance.inputEvents.ChangeInputContext(_previousContext);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        }      
    }

    void UpdateInventory()
    {
        _inventoryLength = _inventorySystem.inventory.Count;
    }

    GameObject[] GetInventoryItemIndex(int displayedItem)
    {
        GameObject[] displayedItems = new GameObject[3];

        if(displayedItem < 0)
        {
            displayedItem = _inventoryLength - 1;
        }
        else if(displayedItem > _inventoryLength - 1)
        {
            displayedItem = 0;
        }

        //Index 0 is always currently viewed object, 1 is previous object, 2 is next object
        if(displayedItem == 0)
        {
            displayedItems[0] = _inventorySystem.inventory[displayedItem].Data.ItemPrefab;
            displayedItems[1] = _inventorySystem.inventory[_inventoryLength - 1].Data.ItemPrefab;
            displayedItems[2] = _inventorySystem.inventory[displayedItem + 1].Data.ItemPrefab;
        }
        else if(displayedItem == _inventoryLength - 1)
        {
            displayedItems[0] = _inventorySystem.inventory[displayedItem].Data.ItemPrefab;
            displayedItems[1] = _inventorySystem.inventory[displayedItem - 1].Data.ItemPrefab;
            displayedItems[2]= _inventorySystem.inventory[0].Data.ItemPrefab;
        }
        else
        {
            displayedItems[0] = _inventorySystem.inventory[displayedItem].Data.ItemPrefab;
            displayedItems[1] = _inventorySystem.inventory[displayedItem- 1].Data.ItemPrefab;
            displayedItems[2] = _inventorySystem.inventory[displayedItem + 1].Data.ItemPrefab;
        }

        _currentlyDisplayedItem = displayedItem;

        return displayedItems;
    }

    void InstantiateInventoryItems(GameObject[]inventoryItems)
    {
        _currentItem = Instantiate(inventoryItems[0],ItemPosition("middle"), Quaternion.Euler(Vector3.zero),_UIContentParent.transform);
        _previousItem = Instantiate(inventoryItems[1], ItemPosition("left"), Quaternion.Euler(Vector3.zero), _UIContentParent.transform);
        _nextItem = Instantiate(inventoryItems[2], ItemPosition("right"), Quaternion.Euler(Vector3.zero), _UIContentParent.transform);

        _currentItem.layer = 5;
        _previousItem.layer = 5;
        _nextItem.layer = 5;
    }

    void SetSlotPositions()
    {
        _slotPositions = new Vector3[5];

        /*Vector3 currentSlotPosition = _UIContentParent.transform.position + new Vector3(-8f, 0f, 5f);

        for(int i = 0; i < 5; i++)
        {
            _slotPositions[i] = currentSlotPosition;

            if(i < 2)
            currentSlotPosition += new Vector3(3f, 0, -2.5f);
            else
            currentSlotPosition += new Vector3(3f, 0, 2.5f);
        }*/
        
        _slotPositions[0] = _UIContentParent.transform.position + new Vector3(-10f, 0f, 3f);
        _slotPositions[1] = _UIContentParent.transform.position + new Vector3(-3.5f, 0f, 1.5f);
        _slotPositions[2] = _UIContentParent.transform.position + new Vector3(0f, 0f, -2f);
        _slotPositions[3] = _UIContentParent.transform.position + new Vector3(3.5f, 0f, 1.5f);
        _slotPositions[4] = _UIContentParent.transform.position + new Vector3(10f, 0f, 3f);
    }

    Vector3 ItemPosition(string position)
    {
        Vector3 newItemPosition = new Vector3();

        switch (position)
        {
            case "OOB_Left":
                newItemPosition = _slotPositions[0];
                break;
            case "left":
                newItemPosition = _slotPositions[1];
                break;
            case "middle":
                newItemPosition = _slotPositions[2];
                break;
            case "right":
                newItemPosition = _slotPositions[3];
                break;
            case "OOB_Right":
                newItemPosition = _slotPositions[4];
                break;
            
        }

        return newItemPosition;
    }

    void DestroyInventoryItems()
    {
        Destroy(_currentItem);
        Destroy(_previousItem);
        Destroy(_nextItem);
    }

    public void ScrollNextItem(InputEventContext context)
    {
        if(context !=  InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight) return;

        GameObject[] newItems = GetInventoryItemIndex(_currentlyDisplayedItem + 1);
        _newItem = Instantiate(newItems[2], _slotPositions[4], Quaternion.Euler(Vector3.zero), _UIContentParent.transform);
        _newItem.layer = 5;

        _moveItemsLeft = true;
    }

    public void ScrollPreviousItem(InputEventContext context )
    {
        if(context !=  InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight) return;

        GameObject[] newItems = GetInventoryItemIndex(_currentlyDisplayedItem - 1);
        _newItem = Instantiate(newItems[1], _slotPositions[0], Quaternion.Euler(Vector3.zero), _UIContentParent.transform);
        _newItem.layer = 5;

        _moveItemsRight = true;
    }

    void ScrollAnimation(string direction)
    {

        Vector3 currentItemDirection;
        Vector3 previousItemDirection;
        Vector3 nextItemDirection;
        Vector3 newItemDirection;

        float adjustedScrollSpeed;

        switch (direction)
        {
            case "next":
                currentItemDirection = _slotPositions[1] - _currentItem.transform.position;
                adjustedScrollSpeed = _scrollSpeed + 0.75f / currentItemDirection.magnitude;
                _currentItem.transform.Translate(currentItemDirection * adjustedScrollSpeed * Time.deltaTime);

                previousItemDirection = _slotPositions[0] - _previousItem.transform.position;
                _previousItem.transform.Translate(previousItemDirection * adjustedScrollSpeed * Time.deltaTime);

                nextItemDirection = _slotPositions[2] - _nextItem.transform.position;
                _nextItem.transform.Translate(nextItemDirection * adjustedScrollSpeed * Time.deltaTime);

                newItemDirection = _slotPositions[3] - _newItem.transform.position;
                _newItem.transform.Translate(newItemDirection * adjustedScrollSpeed * Time.deltaTime);


                if(newItemDirection.magnitude <= 0.01f)
                {
                    _currentItem.transform.position = _slotPositions[1];
                    _previousItem.transform.position = _slotPositions[0];
                    _nextItem.transform.position = _slotPositions[2];
                    _newItem.transform.position = _slotPositions[3];

                    UpdateItemVariables(direction);

                    _moveItemsLeft = false;
                }

                break;

            case "previous":
                currentItemDirection = _slotPositions[3] - _currentItem.transform.position;
                adjustedScrollSpeed = _scrollSpeed + 1 / currentItemDirection.magnitude;
                _currentItem.transform.Translate(currentItemDirection * adjustedScrollSpeed * Time.deltaTime);

                previousItemDirection = _slotPositions[2] - _previousItem.transform.position;
                _previousItem.transform.Translate(previousItemDirection * adjustedScrollSpeed * Time.deltaTime);

                nextItemDirection = _slotPositions[4] - _nextItem.transform.position;
                _nextItem.transform.Translate(nextItemDirection * adjustedScrollSpeed * Time.deltaTime);

                newItemDirection = _slotPositions[1] - _newItem.transform.position;
                _newItem.transform.Translate(newItemDirection * adjustedScrollSpeed * Time.deltaTime);


                if(newItemDirection.magnitude <= 0.01f)
                {
                    _currentItem.transform.position = _slotPositions[3];
                    _previousItem.transform.position = _slotPositions[2];
                    _nextItem.transform.position = _slotPositions[4];
                    _newItem.transform.position = _slotPositions[1];

                    UpdateItemVariables(direction);
                    
                    _moveItemsRight = false;
                }
                break;
        }    
    }

    void UpdateItemVariables(string direction)
    {
        switch (direction)
        {
            case "next":
                Destroy(_previousItem);
                _previousItem = _currentItem;
                _currentItem = _nextItem;
                _nextItem = _newItem;
                _newItem = null;

                break;
            case "previous":
                Destroy(_nextItem);
                _nextItem = _currentItem;
                _currentItem = _previousItem;
                _previousItem = _newItem;
                _newItem = null;
                break;
        }
    }
}
