using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum AbilityType{
    
    PERCENTAGE,
    ABSOLUTE,
    CHANCE,
    DIRECT
}

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager instance;

    public event Action onButtoninteraction;
    public event Action<AbilityButton, bool> onShowButtonText;
    public event Action onUpdateAbilityButtons;


    [SerializeField] TMP_Text _moneyText;

    public int CurrentMoney {get; private set;} = 50;
    int _addedMoney = 0;
    int _displayedMoney = 0;

    public int CurrentPlayerLevel{get; private set;} = 1;

    int _straightModifier = 0;
    float _percentageModifier = 0;
    float _modifierChance = 0;

    public List<PlayerLevelAbilitiesSO> Levels = new List<PlayerLevelAbilitiesSO>(); 
    
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one EXP Managers active!");
            return;
        }

        instance = this;

        _moneyText.gameObject.SetActive(false);

        LoadLevelsSO();
    }

    void LoadLevelsSO()
    {
        Levels.AddRange(Resources.LoadAll<PlayerLevelAbilitiesSO>("AbilityLevels"));
        Debug.Log("Added " + Levels.Count + " Levels!");
    }

    public void AddPlayerLevel()
    {
        CurrentPlayerLevel++;
    }

    public void AddMoney(int amount)
    {
        _moneyText.gameObject.SetActive(true);

        int rawAmount = amount;

        int modifiedAmount = Mathf.FloorToInt(rawAmount * _percentageModifier / 10) + _straightModifier;

        int chancedModifierAmount = (float)UnityEngine.Random.Range(0f, 100f) /10f <= _modifierChance ? rawAmount : 0;

        //_moneyText.text = $"{_currentMoney}" + <color = #E5BF2B>" +" + $"{rawAmount}</color>" + "<color = #E53D2A> +" + $"{modifiedAmount} </color>";

        _addedMoney = rawAmount + modifiedAmount + chancedModifierAmount;

        CurrentMoney += _addedMoney;

        Debug.Log(CurrentMoney);

        UpdateAbilityButtons();

        InvokeRepeating("CountUpMoney", 0, 0.05f);
    }

    public void DirectAddAbsoluteMoney(int amount)
    {
        _moneyText.gameObject.SetActive(true);
        _addedMoney = amount;
        CurrentMoney += _addedMoney;
        InvokeRepeating("CountUpMoney", 0, 0.05f);
    }

    public void ButtonInteraction()
    {
        onButtoninteraction?.Invoke();
    }

    public void UpdateAbilityButtons()
    {
        onUpdateAbilityButtons?.Invoke();
    }

    public void ShowButtonText(AbilityButton button, bool toggle)
    {
        onShowButtonText?.Invoke(button, toggle);
    }

    void CountUpMoney()
    {
        _displayedMoney ++;

        _moneyText.text = $"{_displayedMoney}";

        _addedMoney --;

        if(_addedMoney == 0)
        {
            CancelInvoke();
            StartCoroutine(DelayHideText());

            _displayedMoney = CurrentMoney;
        } 
    }

    public void ChangeAbsoluteModifier(int amount)
    {
        _straightModifier += amount;
    }

    public void ChangePercentageModifier(float amount)
    {
        _percentageModifier += amount;
    }

    public void ChangeModifierChance(float amount)
    {
        _modifierChance += amount;
    }

    IEnumerator DelayHideText()
    {
        yield return new WaitForSeconds(1.5f);

        _moneyText.gameObject.SetActive(false);
    }
}
