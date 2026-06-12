using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    public List<InventoryItem> inventory {get; private set;}

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one InventorySystem active!");
            return;
        }

        instance = this;

        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onPickUpItem += Add;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onPickUpItem -= Add;
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if(m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        //If item is already present in inventory, increase stack, else add new item to inventory and dictionary
        if(m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }

        foreach(InventoryItem item in inventory)
        {
            if(inventory.Count == 0)
            {
               Debug.Log("Inventory empty!");
               return;
            } 

            Debug.Log(item.Data.ItemName);
        }
    }

    //Remove item from stack, if stack size is 0, remove item from inventory
    public void Remove(InventoryItemData referenceData)
    {
        if(m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if(value.StackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
    }
}
