using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class DialogueChoiceButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;

    RectTransform buttonRectTransform;

    float _defaultScaleValue;
    float _targetScaleValue;

    float _scaleSpeedMultiplier;

    bool _isScaleButton;

    private int _choiceIndex = -1;

    private void OnEnable()
    {
        button.onClick.AddListener(() => SelectButton());

        buttonRectTransform = GetComponent<RectTransform>();

        _defaultScaleValue = buttonRectTransform.localScale.x;
    }

    void Update()
    {
        if (_isScaleButton)
        {
            ScaleButton();
        }
    }

    public void SetChoiceText(string choiceTextString)
    {
        choiceText.text = choiceTextString;
    }

    public void SetChoiceIndex(int choiceIndex) 
    { 
        _choiceIndex = choiceIndex;
    }

    public void SelectButton()
    {
        GameEventsManager.instance.dialogueEvents.UpdateChoiceIndex(_choiceIndex);

        _targetScaleValue = _defaultScaleValue * 0.6f;

        _scaleSpeedMultiplier = 1.5f;

        _isScaleButton = true;

        StartCoroutine(DelayChoiceEvent());
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
            buttonRectTransform.localScale = Vector3.MoveTowards(buttonRectTransform.localScale, _targetScaleValue * Vector3.one, 6 * _scaleSpeedMultiplier * Time.deltaTime);

            if(buttonRectTransform.localScale.x == _targetScaleValue)
            {
                _isScaleButton = false;
            }
        }
    }

    private IEnumerator DelayChoiceEvent()
    {
        yield return new WaitForSeconds(0.1f);

        _targetScaleValue = _defaultScaleValue * 1.4f;
        _isScaleButton = true;

        yield return new WaitForSeconds(0.15f);

        _isScaleButton = false;

        GameEventsManager.instance.dialogueEvents.PressedChoiceButton();
    }
}
