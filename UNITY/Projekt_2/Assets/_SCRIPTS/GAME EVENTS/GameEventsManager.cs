using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance {get; private set;}

    public PlayerEvents playerEvents;
    public InputEvents inputEvents;
    public InventoryEvents inventoryEvents;
    public DialogueEvents dialogueEvents;
    public QuestEvents questEvents;
    public InteractionEvents interactionEvents;
    public UIEvents uiEvents;

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
        dialogueEvents = new DialogueEvents();
        questEvents = new QuestEvents();
        interactionEvents = new InteractionEvents();
        uiEvents = new UIEvents();
    }
}
