using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Febucci.TextAnimatorForUnity;
using Unity.VisualScripting;

public class EXP_UI : MonoBehaviour
{
    [SerializeField] List<AbilityButton> _buttons;
    [SerializeField] TypewriterComponent _buttonInfoTypewriter;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Image _levelImageLeft;
    [SerializeField] Image _levelImageRight;

    TMP_Text _lvlLeftText;
    TMP_Text _lvlRightText;

    AbilityButton[] _subLevelOneButtons = new AbilityButton[2];
    AbilityButton[] _subLevelTwoButtons = new AbilityButton[2];
    AbilityButton[] _subLevelThreeButtons = new AbilityButton[2];

    PlayerLevelAbilitiesSO _currentSO;
    int _currentSubLevel = 1;

    bool isUIActive;

    void OnEnable()
    {
       ExperienceManager.instance.onButtoninteraction += ButtonInteraction; 
       ExperienceManager.instance.onShowButtonText += ShowButtonText;
       ExperienceManager.instance.onUpdateAbilityButtons += UpdateAbilityButtons;
    }

    void OnDisable()
    {
        ExperienceManager.instance.onButtoninteraction -= ButtonInteraction; 
        ExperienceManager.instance.onShowButtonText -= ShowButtonText;
        ExperienceManager.instance.onUpdateAbilityButtons -= UpdateAbilityButtons;
    }

    void Start()
    {
        _buttonInfoTypewriter.ShowText("");

        _lvlLeftText = _levelImageLeft.gameObject.GetComponentInChildren<TMP_Text>();
        _lvlRightText = _levelImageRight.gameObject.GetComponentInChildren<TMP_Text>();

        _levelImageLeft.color = new Color(210f / 255, 186f / 255, 51f / 255);

        _lvlLeftText.text = $"{ExperienceManager.instance.CurrentPlayerLevel}";
        _lvlRightText.text = $"{ExperienceManager.instance.CurrentPlayerLevel + 1}";

        AssignButtons();
        UpdateAbilityButtons();
    }

    void AssignButtons()
    {
        _currentSO = ExperienceManager.instance.Levels[ExperienceManager.instance.CurrentPlayerLevel - 1];
        Debug.Log(_currentSO);

        for(int i = 0; i < 6; i++)
        {
            _buttons[i].abilityType = _currentSO.types[i];
            _buttons[i].value = _currentSO.values[i];
            _buttons[i].moneyRequirement = _currentSO.moneyRequirements[i];
        }

        _subLevelOneButtons[0] = _buttons[0];
        _subLevelOneButtons[1] = _buttons[1];
        _subLevelTwoButtons[0] = _buttons[2];
        _subLevelTwoButtons[1] = _buttons[3];
        _subLevelThreeButtons[0] = _buttons[4];
        _subLevelThreeButtons[1] = _buttons[5];
    }

    void ButtonInteraction(AbilityButton button)
    {
        _currentSubLevel++;

        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1, button.gameObject.transform.localPosition);

        if(_currentSubLevel == 4)
        {
            ExperienceManager.instance.AddPlayerLevel();

            _levelImageRight.color = new Color(210f / 255, 186f / 255, 51f / 255);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount-1, new Vector3(350, 0, 0));

            if(ExperienceManager.instance.CurrentPlayerLevel == 10) return;

            StartCoroutine(DelayTreeUpdate());
        }

        UpdateAbilityButtons(); 
    }

    void UpdateAbilityButtons()
    {
        foreach(AbilityButton button in _buttons)
        {
            button.UnlockButton(false);
        }

        switch (_currentSubLevel)
        {
            case 1:
                foreach(AbilityButton button in _subLevelOneButtons)
                {
                    if(button.moneyRequirement <= ExperienceManager.instance.CurrentMoney)
                        button.UnlockButton(true);
                }
                break;
            case 2:
                foreach(AbilityButton button in _subLevelTwoButtons)
                {
                    if(button.moneyRequirement <= ExperienceManager.instance.CurrentMoney)
                        button.UnlockButton(true);
                }
                break;
            case 3:
                foreach(AbilityButton button in _subLevelThreeButtons)
                {
                    if(button.moneyRequirement <= ExperienceManager.instance.CurrentMoney)
                        button.UnlockButton(true);
                }
                break;
        }
    }

    void ShowButtonText(AbilityButton button, bool toggle)
    {
        if (!toggle)
        {
            _buttonInfoTypewriter.ShowText("");
            _buttonInfoTypewriter.gameObject.SetActive(false);
            return;
        }

        string text = "If you see this something went wrong!";

        _buttonInfoTypewriter.gameObject.SetActive(true);
        switch (button.abilityType)
        {
            case AbilityType.PERCENTAGE:
            text = "Every time you gain money, you get " + $"{button.value}" + " percent extra";
                break;
            case AbilityType.ABSOLUTE:
            text = "Every time you gain money, you get " + $"{button.value}" + "$ extra";
                break;
            case AbilityType.CHANCE:
            text = "Every time you gain money, you gain extra " + $"{button.value}" + " percent chance to receive twice as much";
                break;
            case AbilityType.DIRECT:
            text = "Immediately gain " + $"{button.value}" + "$";
                break;
        }

        _buttonInfoTypewriter.ShowText(text);
        _buttonInfoTypewriter.StartShowingText();
    }

    IEnumerator DelayTreeUpdate()
    {
        yield return new WaitForSeconds(1);

        _lvlLeftText.text = $"{ExperienceManager.instance.CurrentPlayerLevel}";
        _lvlRightText.text = $"{ExperienceManager.instance.CurrentPlayerLevel + 1}";
        _levelImageRight.color = new Color(72f / 255, 72f / 255, 72f / 255);

        _lineRenderer.positionCount = 1;

        foreach(AbilityButton button in _buttons)
        {
            ColorBlock buttonColors = button.buttonComponent.colors;
            buttonColors.disabledColor = new Color(72f / 255, 72f / 255, 72f / 255);
            button.buttonComponent.colors = buttonColors;
        }

        _currentSubLevel = 1;
        AssignButtons();

        UpdateAbilityButtons();
    }
}


