using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    [SerializeField] AbilityType abilityType;
    [SerializeField] float value;

    public AbilityButtonLevels_SO levelSO;

    public Button buttonComponent;

    void Start()
    {
        buttonComponent = GetComponent<Button>();

        buttonComponent.onClick.AddListener(() => UnlockAbility());

        UpdateButtonState();
    }

    void OnEnable()
    {
        ExperienceManager.instance.onUpdateAbilityButtons += UpdateButtonState;
    }

    void OnDisable()
    {
        ExperienceManager.instance.onUpdateAbilityButtons -= UpdateButtonState;
    }

    void UnlockAbility()
    {
        if(levelSO.IsLevelUnlocked || ExperienceManager.instance.CurrentMoney < levelSO.MoneyRequirement) return;

        switch (abilityType)
        {
            case AbilityType.PERCENTAGE:
            ExperienceManager.instance.ChangePercentageModifier(value);
                break;
            case AbilityType.ABSOLUTE:
            ExperienceManager.instance.ChangeAbsoluteModifier((int)value);
                break;
            case AbilityType.CHANCE:
            ExperienceManager.instance.ChangeModifierChance(value);
                break;
            case AbilityType.DIRECT:
            ExperienceManager.instance.DirectAddAbsoluteMoney((int)value);
                break;
        }

        levelSO.IsLevelUnlocked = true;

        ExperienceManager.instance.UpdateAbilityButtons();
    }

    void UpdateButtonState()
    {
        //Ability button is interactable if current money is equal or more than required and no other ability on this level was unlocked and if the previous level is unlocked
        if(levelSO.MoneyRequirement <= ExperienceManager.instance.CurrentMoney && !levelSO.IsLevelUnlocked && levelSO.LevelRequirement.IsLevelUnlocked)
        {
            buttonComponent.interactable = true;
        }
        else
        {
            buttonComponent.interactable = false;
        }
    }
}
