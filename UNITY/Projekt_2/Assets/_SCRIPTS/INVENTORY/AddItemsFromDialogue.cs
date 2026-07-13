using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddItemsFromDialogue : MonoBehaviour
{
    [SerializeField] List<InventoryItemData> _possibleItems = new List<InventoryItemData>();

    Dictionary<string, InventoryItemData> items = new Dictionary<string, InventoryItemData>();

    void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onAddItem += AddItemFromDialogue;
        GameEventsManager.instance.inventoryEvents.onRemoveItem += RemoveItemFromDialogue;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onAddItem -= AddItemFromDialogue;
        GameEventsManager.instance.inventoryEvents.onRemoveItem -= RemoveItemFromDialogue;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(InventoryItemData data in _possibleItems)
        {
            items.Add(data.ItemId, data);
        }
    }

    void AddItemFromDialogue(string id)
    {
        GameEventsManager.instance.inventoryEvents.PickUpItem(items[id]);
    }

    void RemoveItemFromDialogue(string id)
    {
        GameEventsManager.instance.inventoryEvents.RemoveItemFromInventory(items[id]);
    }
}
