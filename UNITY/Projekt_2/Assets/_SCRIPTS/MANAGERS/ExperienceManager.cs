using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
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

    public event Action onUpdateAbilityButtons;

    [SerializeField] TMP_Text _moneyText;

    public int CurrentMoney {get; private set;} = 50;
    int _addedMoney = 0;

    public int CurrentPlayerLevel{get; private set;} = 1;

    int _straightModifier = 0;
    float _percentageModifier = 0;
    float _modifierChance = 0;

    public List<AbilityButtonLevels_SO> Levels = new List<AbilityButtonLevels_SO>(); 
    
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
        Levels.AddRange(Resources.LoadAll<AbilityButtonLevels_SO>("AbilityLevels"));
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

        int chancedModifierAmount = (float)UnityEngine.Random.Range(0f, 100f) /10f <= _modifierChance ? modifiedAmount : 0;

        //_moneyText.text = $"{_currentMoney}" + <color = #E5BF2B>" +" + $"{rawAmount}</color>" + "<color = #E53D2A> +" + $"{modifiedAmount} </color>";

        _addedMoney = rawAmount + modifiedAmount + chancedModifierAmount;

        InvokeRepeating("CountUpMoney", 0, 0.05f);
    }

    public void DirectAddAbsoluteMoney(int amount)
    {
        _moneyText.gameObject.SetActive(true);
        _addedMoney = amount;
        InvokeRepeating("CountUpMoney", 0, 0.05f);
    }

    public void UpdateAbilityButtons()
    {
        onUpdateAbilityButtons?.Invoke();
    }

    void CountUpMoney()
    {
        CurrentMoney ++;

        _moneyText.text = $"{CurrentMoney}";

        _addedMoney --;

        if(_addedMoney == 0)
        {
            CancelInvoke();
            StartCoroutine(DelayHideText());
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
