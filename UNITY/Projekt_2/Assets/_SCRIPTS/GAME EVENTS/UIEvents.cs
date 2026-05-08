using System;
using UnityEngine;

public class UIEvents
{
    public event Action<InteractionType> onShowInteractionWidget;

    public void ShowInteractionWidget(InteractionType type)
    {
        onShowInteractionWidget?.Invoke(type);
    }

    public event Action onHideInteractionWidget;

    public void HideInteractionWidget()
    {
        onHideInteractionWidget?.Invoke();
    }
}
