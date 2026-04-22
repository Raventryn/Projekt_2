using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] float _interactionDistance;

    [SerializeField] LayerMask _interactionLayer;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract += Raycast;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract -= Raycast;
    }

    void Raycast(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));

        if(Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayer))
        {
            switch (hit.collider.tag)
            {
                case "Untagged":
                    GameEventsManager.instance.inputEvents.Interaction(InteractionType.DEFAULT, hit.collider.gameObject);
                    break;
                case "PickUp_Object":
                    GameEventsManager.instance.inputEvents.Interaction(InteractionType.PICKUP, hit.collider.gameObject);
                    break;
                case "Dialogue_Object":
                    GameEventsManager.instance.inputEvents.Interaction(InteractionType.DIALOGUE, hit.collider.gameObject);
                    break;
                case "Scanner_Object":
                    GameEventsManager.instance.inputEvents.Interaction(InteractionType.SCANNER, hit.collider.gameObject);
                    break;
            }
            
        }
    }
}
