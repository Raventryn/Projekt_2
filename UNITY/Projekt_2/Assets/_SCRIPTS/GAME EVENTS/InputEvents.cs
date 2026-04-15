using System;
using UnityEditor.Rendering;
using UnityEngine;


public enum InputEventContext
{
    DEFAULT,
    INVENTORY,

    DIALOGUE,
    DIALOGUE_PLAYING
}
public class InputEvents
{
    public InputEventContext Context {get; private set;} = InputEventContext.DEFAULT;

    public void ChangeInputContext(InputEventContext newContext)
    {
        Context = newContext;
    }

    public event Action<InputEventContext> onPressedInteract;

    public void PressedInteract()
    {
        if(onPressedInteract != null)
        {
            onPressedInteract(Context);
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
            onPressedInventory(Context);
        }
    }

    public event Action<InputEventContext> onPressedEscape;

    public void PressedEscape()
    {
        if(onPressedEscape != null)
        {
            onPressedEscape(Context);
        }
    }

    public event Action<bool> onShowCursor;

    public void ShowCursor(bool toggle)
    {
        onShowCursor?.Invoke(toggle);
    }
}
