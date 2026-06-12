using NUnit.Framework;
using UnityEngine;

public class PlayerRaycast : MonoBehaviour
{
    [SerializeField] float _interactionDistance;

    [SerializeField] LayerMask _interactionLayer;

    bool _IsWidgetHidden;
    bool _IsRaycastHit;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract += Raycast;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPressedInteract -= Raycast;
    }

    void Update()
    {
        WidgetRaycast();
    }

    void Raycast(InputEventContext context)
    {
        if(context != InputEventContext.DEFAULT && context != InputEventContext.SCANNER) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));

        if(Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayer))
        {
            if(context == InputEventContext.SCANNER && hit.collider.tag == "Scanner_Object")
            {
                GameEventsManager.instance.inputEvents.Interaction(InteractionType.SCANNER, hit.collider.gameObject);
                return;
            }
            else if(context == InputEventContext.SCANNER) return;

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
                case "Locked_Object":
                    GameEventsManager.instance.inputEvents.Interaction(InteractionType.LOCKED_OBJECT, hit.collider.gameObject);
                    break;
            }
            
        }
    }

    void WidgetRaycast()
    {
        if(GameEventsManager.instance.inputEvents.Context != InputEventContext.DEFAULT && GameEventsManager.instance.inputEvents.Context != InputEventContext.SCANNER) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));

        if(Physics.Raycast(ray, out RaycastHit hit, _interactionDistance, _interactionLayer))
        {
            if (!_IsRaycastHit)
            {
                if(GameEventsManager.instance.inputEvents.Context == InputEventContext.SCANNER && hit.collider.tag == "Scanner_Object")
                {
                    _IsWidgetHidden = false;
                    _IsRaycastHit = true;
                    GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.SCANNER);
                    return;
                }
                else if(GameEventsManager.instance.inputEvents.Context == InputEventContext.SCANNER) return;

                _IsWidgetHidden = false;
                _IsRaycastHit = true;

                switch (hit.collider.tag)
                {
                    case "PickUp_Object":
                        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.PICKUP);
                        break;
                    case "Dialogue_Object":
                        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.DIALOGUE);
                        break;
                    case "Scanner_Object":
                        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.SCANNER);
                        break;
                    case "Locked_Object":
                        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.PICKUP);
                        break;
                }
            }  
        }
        else if (!_IsWidgetHidden)
        {
            GameEventsManager.instance.uiEvents.HideInteractionWidget();
            _IsWidgetHidden = true;
            _IsRaycastHit = false;
        }
    }
}
