using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance {get; private set;}

    public PlayerEvents playerEvents;
    public InputEvents inputEvents;

    public InventoryEvents inventoryEvents;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one Game Events Manager loaded!");
            return;
        }

        instance = this;

        playerEvents = new PlayerEvents();
        inputEvents = new InputEvents();
        inventoryEvents = new InventoryEvents();
    }
}
