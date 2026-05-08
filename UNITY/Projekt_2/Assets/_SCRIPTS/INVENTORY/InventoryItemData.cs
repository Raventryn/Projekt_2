using UnityEngine;

[CreateAssetMenu(menuName = "InventoryItemData", fileName = "NewItem")]
public class InventoryItemData : ScriptableObject
{
    public string ItemId;
    public string ItemName;
    public GameObject ItemPrefab;
    public string ItemInfo;

}
