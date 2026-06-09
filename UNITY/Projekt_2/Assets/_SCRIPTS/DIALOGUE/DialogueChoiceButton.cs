using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueChoiceButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI choiceText;
    Rect buttonRect;

    private int _choiceIndex = -1;

    private void OnEnable()
    {
        button.onClick.AddListener(() => SelectButton());

        buttonRect = button.GetComponent<Rect>();
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

        
    }

    void PulseButton()
    {
        if (buttonRect.Contains(VirtualMouseCursor.instance.CursorScreenPosition))
        {
            
        }
    }

    private IEnumerator DelayChoiceEvent()
    {
        yield return new WaitForSeconds(0.5f);

        GameEventsManager.instance.dialogueEvents.PressedChoiceButton();
    }
}
