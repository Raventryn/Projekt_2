using UnityEngine;
using TMPro;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using UnityEngine.Animations;
using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;
using Febucci.TextAnimatorForUnity;

public class EnterDialogue : MonoBehaviour
{
    [Header("Dialogue Knot")]
    [SerializeField] private string dialogueKnotName;

    [SerializeField] CinemachineCamera _dialogueCamera;

    [SerializeField] GameObject _contentParent;

    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] TextAnimator_TMP dialogueAnimator;
    [SerializeField] TypewriterComponent _dialogueTypewriter;

    [SerializeField] DialogueChoiceButton[] _choiceButtons;

    void Awake()
    {
        ResetPanel();
        _contentParent.SetActive(false);
    }

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onDialogueInteraction += EnterCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape += EarlyExit;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += ExitDialogue;

        GameEventsManager.instance.dialogueEvents.onDialogueFinished += SendFinishedEvent;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onDialogueInteraction -= EnterCamera;
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
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(false);

        GameEventsManager.instance.dialogueEvents.PassDialogueUIPanel(_contentParent, _dialogueTypewriter, dialogueAnimator, _choiceButtons);

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
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);
    }

    private void ResetPanel()
    {
        _dialogueText.text = "";
    }
}
