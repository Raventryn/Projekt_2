using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AbilityType abilityType;
    public float value;
    public int moneyRequirement;

    //public AbilityButtonLevels_SO levelSO;

    public Button buttonComponent;

    void Start()
    {
        buttonComponent = GetComponent<Button>();

        buttonComponent.onClick.AddListener(() => UnlockAbility());
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ShowButtonText(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ShowButtonText(false);
    }

    void UnlockAbility()
    {
        //if(levelSO.IsLevelUnlocked || ExperienceManager.instance.CurrentMoney < levelSO.MoneyRequirement) return;

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

        //levelSO.IsLevelUnlocked = true;

        ExperienceManager.instance.ButtonInteraction();
    }

    void ShowButtonText(bool toggle)
    {
        ExperienceManager.instance.ShowButtonText(this, toggle);
    }

    public void UnlockButton(bool toggle)
    {
        buttonComponent.interactable = toggle;
    }
}
