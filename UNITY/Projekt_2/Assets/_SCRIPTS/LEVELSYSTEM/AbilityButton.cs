using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public AbilityType abilityType;
    public float value;
    public int moneyRequirement;
    public Sprite interactableSprite;
    public Sprite defaultSprite;
    public Color defaultColor;
    public Color black;
    public TMP_Text buttonText;

    public bool isUnlocked;

    //public AbilityButtonLevels_SO levelSO;

    public Button buttonComponent;

    void Start()
    {
        buttonComponent = GetComponent<Button>();

        black = new Color(0, 0, 0);

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

        isUnlocked = true;

        ColorBlock buttonColors = buttonComponent.colors;
        buttonColors.disabledColor = new Color(255f / 255, 205f / 255, 0f / 255);
        buttonComponent.colors = buttonColors;

        ExperienceManager.instance.ButtonInteraction(this);
    }

    void ShowButtonText(bool toggle)
    {
        ExperienceManager.instance.ShowButtonText(this, toggle);
    }

    public void UnlockButton(bool toggle)
    {
        buttonComponent.interactable = toggle;
        if (toggle)
        {
            buttonText.color = Color.black;
        }
        else
        {
            if(isUnlocked)
            {
                buttonText.color = Color.black;
            }
            else
                buttonText.color = Color.white;
        }
    }
}
