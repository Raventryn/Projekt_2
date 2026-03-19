using System;

[Serializable]
public class InventoryItem
{
    public InventoryItemData Data {get; private set;}
    public int StackSize {get; private set;}

    public InventoryItem(InventoryItemData sourceData)
    {
        Data = sourceData;
        AddToStack();
    }

    public void AddToStack()
    {
        StackSize++;
    }

    public void RemoveFromStack()
    {
        StackSize--;
    }
}
