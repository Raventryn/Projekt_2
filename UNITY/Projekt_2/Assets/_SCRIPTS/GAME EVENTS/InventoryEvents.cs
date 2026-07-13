using System;
using UnityEngine;

public class InventoryEvents
{
    public event Action<InventoryItemData> onPickUpItem;

    public void PickUpItem(InventoryItemData referenceItem)
    {
        if(onPickUpItem != null)
        {
            onPickUpItem(referenceItem);
        }
    }

    public event Action<InventoryItemData> onRemoveItemFromInventory;

    public void RemoveItemFromInventory(InventoryItemData referenceItem)
    {
        if(onRemoveItemFromInventory != null)
        {
            onRemoveItemFromInventory(referenceItem);
        }
    }

    public event Action<string> onAddItem;

    public void AddItem(string ItemId)
    {
        onAddItem?.Invoke(ItemId);
    }

    public event Action<string> onRemoveItem;

    public void RemoveItem(string ItemId)
    {
        onRemoveItem?.Invoke(ItemId);
    }
}
