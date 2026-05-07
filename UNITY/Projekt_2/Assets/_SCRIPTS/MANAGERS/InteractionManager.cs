using UnityEngine;

public enum InteractionType
{
    DEFAULT,
    PICKUP,
    DIALOGUE,
    SCANNER
}

public class InteractionManager : MonoBehaviour
{
    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onInteraction += SendInteractionEvents;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onInteraction -= SendInteractionEvents;
    }

    void SendInteractionEvents(InteractionType type, GameObject gameObject)
    {
        switch (type)
        {
            case InteractionType.DEFAULT:
                GameEventsManager.instance.interactionEvents.Interaction(gameObject);
                break;
            case InteractionType.PICKUP:
                GameEventsManager.instance.interactionEvents.PickUpInteraction(gameObject);
                break;
            case InteractionType.DIALOGUE:
                GameEventsManager.instance.interactionEvents.DialogueInteraction(gameObject);
                break;
            /*case InteractionType.SCANNER:
                GameEventsManager.instance.interactionEvents.ScannerInteraction(gameObject);
                break;*/
        }
    }
}
