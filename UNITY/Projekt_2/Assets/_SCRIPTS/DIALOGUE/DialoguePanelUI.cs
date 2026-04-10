using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Ink.Runtime;
using Ink.Parsed;
using UnityEngine.UI;

public class DialoguePanelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;


    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;

        GameEventsManager.instance.dialogueEvents.onPassDialogueUIPanel += SetReferences;
        GameEventsManager.instance.dialogueEvents.onClearDialogueUIPanel += ClearReferences;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;

        GameEventsManager.instance.dialogueEvents.onPassDialogueUIPanel += SetReferences;
        GameEventsManager.instance.dialogueEvents.onClearDialogueUIPanel += ClearReferences;
    }

    private void SetReferences(GameObject contentParent, TextMeshProUGUI dialogueText, DialogueChoiceButton[] choiceButtons)
    {
        this.contentParent = contentParent;
        this.dialogueText = dialogueText;
        this.choiceButtons = choiceButtons;
    }

    private void ClearReferences()
    {
        contentParent = null;
        dialogueText = null;
        choiceButtons = null;
    }

    private void DialogueStarted()
    {
        contentParent.SetActive(true);
        //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.UI_OPEN);
    }

    private void DialogueFinished()
    {
        contentParent.SetActive(false);
        //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.UI_OPEN);
    }

    private void DisplayDialogue(string dialogueLine, List<Ink.Runtime.Choice> dialogueChoices)
    {
        dialogueText.text = dialogueLine;

        if(dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("More Dialogue Choices ("
                + dialogueChoices.Count + ") came through than are supported ("
                + choiceButtons.Length + ")");
        }

        foreach(DialogueChoiceButton choiceButton in choiceButtons) 
        {
            choiceButton.gameObject.SetActive(false);
        }

        int choiceButtonIndex = dialogueChoices.Count - 1;
        for(int inkChoiceIndex = 0;  inkChoiceIndex < dialogueChoices.Count; inkChoiceIndex++)
        {
            Ink.Runtime.Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
            DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(dialogueChoice.text);
            choiceButton.SetChoiceIndex(inkChoiceIndex);

            choiceButtonIndex--;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    
}
