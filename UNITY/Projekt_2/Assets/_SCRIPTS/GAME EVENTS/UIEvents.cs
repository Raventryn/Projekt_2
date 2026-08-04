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

    public event Action<bool> onShowScannerTooltip;

    public void ShowScannerTooltip(bool toggle)
    {
        onShowScannerTooltip?.Invoke(toggle);
    }

    public event Action<bool> onShowInventoryTooltip;

    public void ShowInventoryTooltip(bool toggle)
    {
        onShowInventoryTooltip?.Invoke(toggle);
    }

    public event Action onGlitchOnMenu;

    public void GlitchOnMenu()
    {
        onGlitchOnMenu?.Invoke();
    }

    public event Action onQuitGame;

    public void QuitGame()
    {
        onQuitGame?.Invoke();
    }
}
