using System.Collections.Generic;
using System.Security.Cryptography;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InteractionIconManager : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] List <InteractionType> _spriteContext;
    [SerializeField] float scaleSpeed;
    Dictionary<InteractionType, Sprite> _interactionSprites = new Dictionary<InteractionType, Sprite>();

    float targetScaleValue;
    bool isIconShown;
    bool isScalingIcon;

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
        targetScaleValue = _image.gameObject.transform.localScale.x;

        _image.enabled = false;
    }

    void Update()
    {
        if (isIconShown && isScalingIcon)
        {
            ScaleUpObject();
        }
        else if(!isIconShown && isScalingIcon)
        {
            ScaleDownObject();
        }
    }

    void ShowInteractionWidget(InteractionType type)
    {
        _image.sprite = _interactionSprites[type];

        _image.enabled = true;

        isIconShown = true;
        isScalingIcon = true;
    }

    void HideInteractionWidget()
    {
        isIconShown = false;
        isScalingIcon = true;
        //hiding image in scale method
    }
    void ScaleUpObject()
    {
        if(_image.gameObject.transform.localScale.x < targetScaleValue)
        {
            _image.gameObject.transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;

            if(_image.gameObject.transform.localScale.x >= targetScaleValue)
            {
                _image.gameObject.transform.localScale = Vector3.one * targetScaleValue;
                isScalingIcon = false;
            }
        }
    }
    void ScaleDownObject()
    {
        if(_image.gameObject.transform.localScale.x > 0)
        {
            _image.gameObject.transform.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;

            if(_image.gameObject.transform.localScale.x <= 0)
            {
                _image.gameObject.transform.localScale = Vector3.zero;
                isScalingIcon = false;

                _image.enabled = false;
            }
        }
    }
}
