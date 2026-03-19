using System;
using UnityEngine;


public enum InputEventContext
{
    DEFAULT,
    INVENTORY
}
public class InputEvents
{
    private InputEventContext context = InputEventContext.DEFAULT;
    public void ChangeInputContext(InputEventContext newContext)
    {
        context = newContext;
    }

    public event Action<InputEventContext> onPressedInteract;

    public void PressedInteract()
    {
        if(onPressedInteract != null)
        {
            onPressedInteract(context);
        }
    }

    public event Action<GameObject> onInteraction;

    public void Interaction(GameObject gameObject)
    {
        if(onInteraction != null)
        {
            onInteraction(gameObject);
        }
    }

    public event Action<InputEventContext> onPressedInventory;

    public void PressedInventory()
    {
        if(onPressedInteract != null)
        {
            onPressedInventory(context);
        }
    }
}
