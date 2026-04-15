using UnityEngine;
using TMPro;
using UnityEngine.Animations;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnterDialogue : MonoBehaviour
{
    [Header("Dialogue Knot")]
    [SerializeField] private string dialogueKnotName;

    [SerializeField] CinemachineCamera _dialogueCamera;

    [SerializeField] GameObject _contentParent;

    [SerializeField] TextMeshProUGUI _dialogueText;

    [SerializeField] DialogueChoiceButton[] _choiceButtons;

    void Awake()
    {
        ResetPanel();
        _contentParent.SetActive(false);
    }

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onInteraction += EnterCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape += EarlyExit;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += ExitDialogue;

        GameEventsManager.instance.dialogueEvents.onDialogueFinished += SendFinishedEvent;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onInteraction -= EnterCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape -= EarlyExit;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= ExitDialogue;

        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= SendFinishedEvent;
    }

    public void SendDialogueEvent()
    {
        if (!dialogueKnotName.Equals(""))
        {
            GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueKnotName);
        }
    }

    private void SendFinishedEvent()
    {
        //GameEventsManager.instance.npcEvents.ResumeBehaviour(gameObject);
    }

    void EnterCamera(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;
        
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);
        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(false);

        GameEventsManager.instance.dialogueEvents.PassDialogueUIPanel(_contentParent, _dialogueText, _choiceButtons);

        _dialogueCamera.Priority = 1;

        SendDialogueEvent();
    }

    void EarlyExit(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE) return;

        ExitDialogue();
    }

    void ExitDialogue()
    {
        _dialogueCamera.Priority = -1;

        _dialogueCamera.LookAt = null;

        GameEventsManager.instance.dialogueEvents.ClearDialogueUIPanel();

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(true);
        GameEventsManager.instance.playerEvents.LockPlayerCamera(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);
    }

    private void ResetPanel()
    {
        _dialogueText.text = "";
    }
}
