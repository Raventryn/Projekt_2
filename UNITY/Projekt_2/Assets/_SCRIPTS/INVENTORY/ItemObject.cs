using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onInteraction += PickUpObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onInteraction -= PickUpObject;
    }

    public void PickUpObject(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        GameEventsManager.instance.inventoryEvents.PickUpItem(referenceItem);
        Destroy(gameObject);
    }
}
