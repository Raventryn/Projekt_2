using UnityEngine;
using System.Collections;

public class ScaleButtonOnHover : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 4;

    RectTransform buttonRectTransform;

    float _defaultScaleValue;
    float _targetScaleValue;
    float _scaleSpeedMultiplier;

    bool _isScaleButton;

    void OnEnable()
    {
        buttonRectTransform = GetComponent<RectTransform>();

        _defaultScaleValue = buttonRectTransform.localScale.x;
        
        buttonRectTransform.localScale = _defaultScaleValue * Vector3.one;
    }

    void OnDisable()
    {
        buttonRectTransform.localScale = _defaultScaleValue * Vector3.one;
        _isScaleButton = false;
    }

    void Update()
    {
        if (_isScaleButton)
        {
            ScaleButton();
        }
    }

    public void PulseButton(int value)
    {

        switch (value)
        {
            case 1:
                _targetScaleValue = _defaultScaleValue * 1.4f;
                break;
            case -1:
                _targetScaleValue = _defaultScaleValue;
                break;
        }

        _scaleSpeedMultiplier = 1;

        _isScaleButton = true;
    }

    void ScaleButton()
    {
        if(buttonRectTransform.localScale.x != _targetScaleValue)
        {
            buttonRectTransform.localScale = Vector3.MoveTowards(buttonRectTransform.localScale, _targetScaleValue * Vector3.one, scaleSpeed * _scaleSpeedMultiplier * Time.deltaTime);

            if(buttonRectTransform.localScale.x == _targetScaleValue)
            {
                _isScaleButton = false;
            }
        }
    }

    public void SelectButton()
    {
        _targetScaleValue = _defaultScaleValue * 0.6f;

        _scaleSpeedMultiplier = 3f;

        _isScaleButton = true;

        StartCoroutine(DelayChoiceEvent());
    }

    private IEnumerator DelayChoiceEvent()
    {
        yield return new WaitForSeconds(0.1f);

        _targetScaleValue = _defaultScaleValue * 1.4f;
        _isScaleButton = true;

        yield return new WaitForSeconds(0.15f);

        _isScaleButton = false;
    }
}
