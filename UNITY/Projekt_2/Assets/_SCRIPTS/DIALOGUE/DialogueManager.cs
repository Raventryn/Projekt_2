using UnityEngine;
using Ink.Runtime;
using System.Collections;
using UnityEngine.InputSystem;


//https://www.youtube.com/watch?v=l8yI_97vjZs&t=1360
public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    private Story _story;

    private InkExternalFunctions inkExternalFunctions;

    private InkDialogueVariables inkDialogueVariables;

    private int currentChoiceIndex = -1;

    private bool _dialoguePlaying = false;


    private void Awake()
    {
        _story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(_story);
        inkDialogueVariables = new InkDialogueVariables(_story);
    }

    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(_story);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.onPressedInteract += PressedInteract;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onPressedChoiceButton += PressedButton;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialoguevariable += UpdateInkDialogueVariable;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.onPressedInteract -= PressedInteract;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onPressedChoiceButton -= PressedButton;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialoguevariable -= UpdateInkDialogueVariable;
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
       this.currentChoiceIndex = choiceIndex;
    }

    private void PressedInteract(InputEventContext context)
    {
        if (context != InputEventContext.DIALOGUE) return;
        else
        {
            if(_story.currentChoices.Count == 0)
            ContinueOrExitStory();
        }
    }

    private void PressedButton()
    {
        if (_story.currentChoices.Count <= 0) return;
        else
        {
            if (_story.currentChoices.Count > 0)
            //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.UI_CLICK);
                ContinueOrExitStory();
        }
    }

    private void EnterDialogue(string knotName)
    {
        if (_dialoguePlaying) return;

        _dialoguePlaying = true;

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);

        if (!knotName.Equals(""))
        {
            _story.ChoosePathString(knotName);

            Debug.Log(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was empty string when entering Dialogue");
        }

        GameEventsManager.instance.dialogueEvents.DialogueStarted();

        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);

        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);

        inkDialogueVariables.SyncVariablesAndStartListening(_story);

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory()
    {
        if(_story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            _story.ChooseChoiceIndex(currentChoiceIndex);

            currentChoiceIndex = -1;
        }

        if (_story.canContinue)
        {
            string dialogueLine = _story.Continue();

            while (IsLineBlank(dialogueLine) && _story.canContinue)
            {
                dialogueLine = _story.Continue();
            }

            if(IsLineBlank(dialogueLine) && !_story.canContinue)
            {
                ExitDialogue();
            }
            else
            {
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, _story.currentChoices);
            }     
        }
        else if(_story.currentChoices.Count == 0)
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        _dialoguePlaying = false;

        inkDialogueVariables.StopListening(_story);

        _story.ResetState();

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);

        Debug.Log("Exit Dialogue");

        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        //GameEventsManager.instance.uiEvents.SendIteractionSprite(UI_Widget.TALK);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }

}
