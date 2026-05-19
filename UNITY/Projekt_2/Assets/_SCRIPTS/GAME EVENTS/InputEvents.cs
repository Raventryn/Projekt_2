using System;
using UnityEditor.Rendering;
using UnityEngine;


public enum InputEventContext
{
    DEFAULT,
    INVENTORY,

    DIALOGUE,
    DIALOGUE_PLAYING,
    SCANNER,
    SCANNER_VIEW,
    SCANNER_BUTTONS,
    SCANNER_MINIGAME,
    CALIBRATING
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

    public event Action<InputEventContext> onHoldInteract;

    public void HoldInteract()
    {
        onHoldInteract?.Invoke(Context);
    }

    public event Action<InputEventContext> onReleaseInteract;

    public void ReleaseInteract()
    {
        onReleaseInteract?.Invoke(Context);
    }

    public event Action<InteractionType, GameObject> onInteraction;

    public void Interaction(InteractionType type, GameObject gameObject)
    {
        if(onInteraction != null)
        {
            onInteraction(type, gameObject);
        }
    }

    public event Action<InputEventContext> onPressedAltInteract;

    public void PressedAltInteract()
    {
        onPressedAltInteract?.Invoke(Context);
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

    public event Action<InputEventContext> onWalkLeft;

    public void WalkLeft()
    {
        onWalkLeft?.Invoke(Context);
    }

    public event Action<InputEventContext> onWalkRight;

    public void WalkRight()
    {
        onWalkRight?.Invoke(Context);
    }

    public event Action<InputEventContext> onWalkUp;

    public void WalkUp()
    {
        onWalkUp?.Invoke(Context);
    }

    public event Action<InputEventContext> onWalkDown;

    public void WalkDown()
    {
        onWalkDown?.Invoke(Context);
    }

    public event Action<InputEventContext, float> onEquipScanner;

    public void EquipScanner(float value)
    {
        onEquipScanner?.Invoke(Context, value);
    }

    public event Action<InputEventContext, Vector2> onLook;

    public void Look(Vector2 value)
    {
        onLook?.Invoke(Context, value);
    }
}
