using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionIconManager : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] List <InteractionType> _spriteContext;
    Dictionary<InteractionType, Sprite> _interactionSprites = new Dictionary<InteractionType, Sprite>();

    void OnEnable()
    {
        GameEventsManager.instance.uiEvents.onShowInteractionWidget += ShowInteractionWidget;
        GameEventsManager.instance.uiEvents.onHideInteractionWidget += HideInteractionWidget;
    }

    void OnDisable()
    {
        GameEventsManager.instance.uiEvents.onShowInteractionWidget -= ShowInteractionWidget;
        GameEventsManager.instance.uiEvents.onHideInteractionWidget -= HideInteractionWidget;
    }

    void Start()
    {
         int i = 0;

        foreach(Sprite sprite in _sprites)
        {
            _interactionSprites.Add(_spriteContext[i], _sprites[i]);

            i++;
        }

        _image.enabled = false;
    }

    void ShowInteractionWidget(InteractionType type)
    {
        _image.sprite = _interactionSprites[type];

        _image.enabled = true;
    }

    void HideInteractionWidget()
    {
        _image.enabled = false;
    }
}
