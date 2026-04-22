using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onPickUpInteraction += PickUpObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onPickUpInteraction -= PickUpObject;
    }

    public void PickUpObject(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        GameEventsManager.instance.inventoryEvents.PickUpItem(referenceItem);
        Destroy(gameObject);
    }
}
