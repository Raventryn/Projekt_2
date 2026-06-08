using System.Collections;
using System.Collections.Generic;
using Febucci.TextAnimatorForUnity;
using TMPro;
using UnityEditor.EditorTools;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] InventorySystem _inventorySystem;
    [SerializeField] Canvas _uiCanvas;
    [SerializeField] GameObject _InventoryContentParent;
    [SerializeField] GameObject _LevelMenuContentParent;
    [SerializeField] float _scrollSpeed = 2;

    [Tooltip("GameObject with TMP_Text and Typewriter component for Item Name")]
    [SerializeField] GameObject _itemName;
    [Tooltip("GameObject with TMP_Text and Typewriter component for Item Info")]
    [SerializeField] GameObject _itemInfo;

    TMP_Text _itemNameText;
    TypewriterComponent _itemNameTypewriter;
    TMP_Text _itemInfoText;
    TypewriterComponent _itemInfoTypewriter;
    [SerializeField] Button _leftScrollButton;
    [SerializeField] Button _rightScrollbutton;
    [SerializeField] Button _switchMenusButton;


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

    bool _itemsOnLeftSide = true;


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

        _itemNameText = _itemName.GetComponent<TMP_Text>();
        _itemNameTypewriter = _itemName.GetComponent<TypewriterComponent>();
        _itemInfoText = _itemInfo.GetComponent<TMP_Text>();
        _itemInfoTypewriter = _itemInfo.GetComponent<TypewriterComponent>();

        _leftScrollButton.interactable = false;
        _rightScrollbutton.interactable = false;

        _switchMenusButton.onClick.AddListener(() => SwitchUIMenu());

        _InventoryContentParent.SetActive(false);
        _LevelMenuContentParent.SetActive(false);
        ClearItemTexts();

        //_uiCanvas.gameObject.SetActive(false);
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

    void SetItemTexts(InventoryItemData data)
    {
        _itemNameTypewriter.ShowText(data.ItemName);
        _itemInfoTypewriter.ShowText(data.ItemInfo);

        _itemNameTypewriter.StartShowingText();
        _itemInfoTypewriter.StartShowingText();
    }

    void ClearItemTexts()
    {
        _itemNameText.text = "";
        _itemInfoText.text = "";
    }

    void ToggleUI(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight) return;
        
        _isUIenabled = !_isUIenabled;

        if(_isUIenabled == true)
        {
            _previousContext = context;

            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.INVENTORY);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

            GameEventsManager.instance.inputEvents.ShowCursor(true);
            Debug.Log(Cursor.lockState);
        }
        else
        {
            GameEventsManager.instance.inputEvents.ChangeInputContext(_previousContext);

            GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
            GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);

            GameEventsManager.instance.inputEvents.ShowCursor(false);
        }

        ToggleInventory(_isUIenabled);
        _LevelMenuContentParent.SetActive(false);

        //_uiCanvas.gameObject.SetActive(_isUIenabled);
    }

    void ToggleInventory(bool toggle)
    {
        _InventoryContentParent.SetActive(toggle);

        if (_InventoryContentParent.activeSelf)
        {
            UpdateInventory();
            if(_inventoryLength > 1)
            {
                _leftScrollButton.interactable = true;
                _rightScrollbutton.interactable = true;
            }    
            else if( _inventoryLength <= 1)
            {
                _leftScrollButton.interactable = false;
                _rightScrollbutton.interactable = false;
            }
            if(_inventoryLength > 0) InstantiateInventoryItems(GetInventoryItems(_currentlyDisplayedItem));

            //Debug.Log("Current: " + _currentItem.name);
        }
        else
        {
            DestroyInventoryItems();
            ClearItemTexts();
        }   
    }

    void ToggleLevelMenu(bool toggle)
    {
        _LevelMenuContentParent.SetActive(toggle);
    }

    void SwitchUIMenu()
    {
        if (_InventoryContentParent.activeSelf)
        {
            ToggleInventory(false);
            ToggleLevelMenu(true);
        }
        else
        {
            ToggleInventory(true);
            ToggleLevelMenu(false);
        }
    }

    void UpdateInventory()
    {
        _inventoryLength = _inventorySystem.inventory.Count;
    }


    //Returns index for previous and next items, based on current position in inventory
    int[] GetInventoryIndexes(int displayedItem)
    {
        int[] indexes = new int[2];

        //If current item is first in inventory list, previous item will be last in list
        if(displayedItem == 0)
        {
            indexes[0] = _inventoryLength - 1;
            indexes[1] = displayedItem + 1;
        }
        //If current item is last in inventory list, next item will be first in list
        else if(displayedItem == _inventoryLength - 1)
        {
            indexes[0] = displayedItem - 1;
            indexes[1] = 0;
        }
        else
        {
            indexes[0] = displayedItem - 1;
            indexes[1] = displayedItem + 1;
        }

        return indexes;
    }

    //Returns List of items to instantiate
    List<InventoryItemData> GetInventoryItems(int displayedItem)
    {
        List<InventoryItemData> displayedItems = new List<InventoryItemData>();

        if(displayedItem < 0)
        {
            displayedItem = _inventoryLength - 1;
        }
        else if(displayedItem > _inventoryLength - 1)
        {
            displayedItem = 0;
        }

        for(int i = 0; i < _inventoryLength; i++)
        {
            switch(i)
            {
                case 0:
                    displayedItems.Add(_inventorySystem.inventory[displayedItem].Data);
                    break;
                case 1:
                    displayedItems.Add(_inventorySystem.inventory[GetInventoryIndexes(displayedItem)[0]].Data);
                    break;
                case 2:
                    displayedItems.Add(_inventorySystem.inventory[GetInventoryIndexes(displayedItem)[1]].Data);
                    break;
            }
        }
    
        _currentlyDisplayedItem = displayedItem;

        SetItemTexts(displayedItems[0]);

        return displayedItems;
    }

    void InstantiateInventoryItems(List<InventoryItemData>inventoryItems)
    {
        if(inventoryItems.Count == 0)
        {
            //Display some notification or similar that inventory is empty
            return; 
        }

        if(inventoryItems.Count == 1)
        {
            _currentItem = Instantiate(inventoryItems[0].ItemPrefab,ItemPosition("middle"), Quaternion.Euler(Vector3.zero),_InventoryContentParent.transform);
            _currentItem.layer = 5;
        }
        else if(inventoryItems.Count == 2)
        {
            _currentItem = Instantiate(inventoryItems[0].ItemPrefab,ItemPosition("middle"), Quaternion.Euler(Vector3.zero),_InventoryContentParent.transform);
            _previousItem = Instantiate(inventoryItems[1].ItemPrefab, ItemPosition("left"), Quaternion.Euler(Vector3.zero), _InventoryContentParent.transform);
            _nextItem = _previousItem;

            _currentItem.layer = 5;
            _previousItem.layer = 5;
        }
        else
        {
            _currentItem = Instantiate(inventoryItems[0].ItemPrefab,ItemPosition("middle"), Quaternion.Euler(Vector3.zero),_InventoryContentParent.transform);
            _previousItem = Instantiate(inventoryItems[1].ItemPrefab, ItemPosition("left"), Quaternion.Euler(Vector3.zero), _InventoryContentParent.transform);
            _nextItem = Instantiate(inventoryItems[2].ItemPrefab, ItemPosition("right"), Quaternion.Euler(Vector3.zero), _InventoryContentParent.transform);

            _currentItem.layer = 5;
            _previousItem.layer = 5;
            _nextItem.layer = 5;  
        }  
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
        
        _slotPositions[0] = _InventoryContentParent.transform.position + new Vector3(-10f, 0f, 3f);
        _slotPositions[1] = _InventoryContentParent.transform.position + new Vector3(-3.5f, 0f, 1.5f);
        _slotPositions[2] = _InventoryContentParent.transform.position + new Vector3(0f, 0f, -2f);
        _slotPositions[3] = _InventoryContentParent.transform.position + new Vector3(3.5f, 0f, 1.5f);
        _slotPositions[4] = _InventoryContentParent.transform.position + new Vector3(10f, 0f, 3f);
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

    public void ScrollButton(string direction)
    {
        switch (direction)
        {
            case "left":
                ScrollPreviousItem(InputEventContext.INVENTORY);
                break;
            case "right":
                ScrollNextItem(InputEventContext.INVENTORY);
                break;
        }
    }

    public void ScrollNextItem(InputEventContext context)
    {
        if(context !=  InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight || _inventoryLength <= 1) return;

        ClearItemTexts();

        List<InventoryItemData> newItems;

        _currentlyDisplayedItem += 1;

        newItems = GetInventoryItems(_currentlyDisplayedItem);

        SetItemTexts(newItems[0]);

        if(_inventoryLength >= 3)
        {
            _newItem = Instantiate(newItems[2].ItemPrefab, _slotPositions[4], Quaternion.Euler(Vector3.zero), _InventoryContentParent.transform);
            _newItem.layer = 5;
        }
        
        _moveItemsLeft = true;
    }

    public void ScrollPreviousItem(InputEventContext context )
    {
        if(context !=  InputEventContext.INVENTORY || _moveItemsLeft || _moveItemsRight || _inventoryLength <= 1) return;

        ClearItemTexts();

        List<InventoryItemData> newItems;

        _currentlyDisplayedItem -= 1;

        newItems = GetInventoryItems(_currentlyDisplayedItem);

        SetItemTexts(newItems[0]);

        if(_inventoryLength >= 3)
        {
            //newItems = GetInventoryItems(_currentlyDisplayedItem);
            _newItem = Instantiate(newItems[1].ItemPrefab, _slotPositions[0], Quaternion.Euler(Vector3.zero), _InventoryContentParent.transform);
            _newItem.layer = 5;
        }

        _moveItemsRight = true;
    }

    void ScrollAnimation(string direction)
    {

        Vector3 currentItemDirection;
        Vector3 previousItemDirection;
        Vector3 nextItemDirection;
        Vector3 newItemDirection;

        float adjustedScrollSpeed;

        if(_inventoryLength <= 2)
        {

            switch (_itemsOnLeftSide)
            {
                case true:
                    currentItemDirection = _slotPositions[3] - _currentItem.transform.position;
                    adjustedScrollSpeed = _scrollSpeed + 0.75f / currentItemDirection.magnitude;
                    _currentItem.transform.Translate(currentItemDirection * adjustedScrollSpeed * Time.deltaTime);

                    previousItemDirection = _slotPositions[2] - _previousItem.transform.position;
                    _previousItem.transform.Translate(previousItemDirection * adjustedScrollSpeed * Time.deltaTime);

                    if(currentItemDirection.magnitude <= 0.01f)
                    {
                    _currentItem.transform.position = _slotPositions[3];
                    _previousItem.transform.position = _slotPositions[2];

                    GameObject placeholder = _currentItem;
                    _currentItem = _previousItem;
                    _previousItem = placeholder;
                    
                    _moveItemsLeft = false;
                    _moveItemsRight = false;
                    }
                    break;

                case false:
                    currentItemDirection = _slotPositions[1] - _currentItem.transform.position;
                    adjustedScrollSpeed = _scrollSpeed + 0.75f / currentItemDirection.magnitude;
                    _currentItem.transform.Translate(currentItemDirection * adjustedScrollSpeed * Time.deltaTime);

                    previousItemDirection = _slotPositions[2] - _previousItem.transform.position;
                    _previousItem.transform.Translate(previousItemDirection * adjustedScrollSpeed * Time.deltaTime);


                    if(currentItemDirection.magnitude <= 0.01f)
                    {
                    _currentItem.transform.position = _slotPositions[1];
                    _previousItem.transform.position = _slotPositions[2];

                    GameObject placeholder = _currentItem;
                    _currentItem = _previousItem;
                    _previousItem = placeholder;
                
                    
                    _moveItemsRight = false;
                    _moveItemsLeft = false;
                    }
                    break;
            }

            if(!_moveItemsLeft && !_moveItemsRight)
            {
                _itemsOnLeftSide = !_itemsOnLeftSide;
            }
        }
        else
        {
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
