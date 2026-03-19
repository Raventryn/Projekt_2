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
}
