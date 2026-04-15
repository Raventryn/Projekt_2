using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Ink.Runtime;
using Ink.Parsed;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DialoguePanelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;

    private bool _dialoguePlaying = false;

    private string _dialogueLine;


    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;

        GameEventsManager.instance.dialogueEvents.onPassDialogueUIPanel += SetReferences;
        GameEventsManager.instance.dialogueEvents.onClearDialogueUIPanel += ClearReferences;

        GameEventsManager.instance.inputEvents.onPressedInteract += SkipDialogue;
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
        if(!dialogueLine.Contains("<br>"))
        {
            _dialogueLine = dialogueLine;
            StartCoroutine(ShowChars(dialogueLine));
        }
        else
        {
            dialogueText.text = dialogueLine;
        }

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
    

    private void SkipDialogue(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE_PLAYING) return;

        if(!_dialoguePlaying) return;

        StopAllCoroutines();

        dialogueText.text = _dialogueLine;

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
    }


    private IEnumerator ShowChars(string bufferText)
    {
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE_PLAYING);

        _dialoguePlaying = true;

        string dialogueLine = "";

        foreach(char character in bufferText)
        {
            dialogueLine += character;
            yield return new WaitForSeconds(0.03f);
            dialogueText.text = dialogueLine;
        }

        _dialoguePlaying = false;

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
    }
}
