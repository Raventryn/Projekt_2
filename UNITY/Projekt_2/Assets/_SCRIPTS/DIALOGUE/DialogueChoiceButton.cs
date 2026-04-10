using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DialogueChoiceButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;

    private int _choiceIndex = -1;

    private void OnEnable()
    {
        button.onClick.AddListener(() => SelectButton());
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

        GameEventsManager.instance.dialogueEvents.PressedChoiceButton();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
