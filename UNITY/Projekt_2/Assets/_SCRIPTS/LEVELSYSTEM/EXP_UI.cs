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

       GameEventsManager.instance.inputEvents.onPressedInventory += PlaceholderShowUI;
    }

    void OnDisable()
    {
        ExperienceManager.instance.onButtoninteraction -= ButtonInteraction; 
        ExperienceManager.instance.onShowButtonText -= ShowButtonText;
        ExperienceManager.instance.onUpdateAbilityButtons -= UpdateAbilityButtons;

        GameEventsManager.instance.inputEvents.onPressedInventory -= PlaceholderShowUI;
    }

    void Start()
    {
        AssignButtons();
    }

    void PlaceholderShowUI(InputEventContext context)
    {
        isUIActive = !isUIActive;

        if (isUIActive)
        {
            UpdateAbilityButtons();
        }
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

    void ButtonInteraction()
    {
        _currentSubLevel++;

        if(_currentSubLevel == 4)
        {
            ExperienceManager.instance.AddPlayerLevel();
            _currentSubLevel = 1;
            AssignButtons();
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
            text = "Every time you gain money, you gain extra" + $"{button.value}" + " percent chance to receive twice as much";
                break;
            case AbilityType.DIRECT:
            text = "Immediately gain " + $"{button.value}" + "$";
                break;
        }

        _buttonInfoTypewriter.ShowText(text);
        _buttonInfoTypewriter.StartShowingText();
    }
}


