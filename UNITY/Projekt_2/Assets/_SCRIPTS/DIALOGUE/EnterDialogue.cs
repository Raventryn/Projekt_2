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

    Vector3 _cameraDefaultPosition;
    float _offsetClampValue;
    bool IsInDialogue;

    void Awake()
    {
        ResetPanel();
        _contentParent.SetActive(false);
        _cameraDefaultPosition = _dialogueCamera.transform.localPosition;
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

    void Update()
    {
        if (IsInDialogue && VirtualMouseCursor.instance.IsCursorVisible)
        {
            OffsetScanViewCamera();
        }
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

        _offsetClampValue = (this.gameObject.transform.position - _dialogueCamera.transform.position).magnitude;
        Debug.Log(_offsetClampValue);

        IsInDialogue = true;

        SendDialogueEvent();
    }

    void OffsetScanViewCamera()
    {
        Vector2 screenUV = new Vector2((VirtualMouseCursor.instance.CursorScreenPosition.x / Screen.width) -0.5f, (VirtualMouseCursor.instance.CursorScreenPosition.y / Screen.height) -0.5f);

        Debug.Log(screenUV);

        float xPos = _cameraDefaultPosition.z + (screenUV.x / (5f * _offsetClampValue));//Mathf.Clamp(_camera.transform.localPosition.x + (screenUV.x / 5), _cameraDefaultPosition.x - 0.1f, _cameraDefaultPosition.x + 0.1f);
        float yPos = _cameraDefaultPosition.y + (screenUV.y / (5f * _offsetClampValue));//Mathf.Clamp(_camera.transform.localPosition.z + (screenUV.y / 5), _cameraDefaultPosition.z - 0.1f, _cameraDefaultPosition.z + 0.1f);

        Vector3 newPosition = new Vector3(_cameraDefaultPosition.x, yPos, xPos);

        _dialogueCamera.transform.localPosition = Vector3.MoveTowards(_dialogueCamera.transform.localPosition, newPosition, 0.1f + Time.deltaTime);
    }

    void EarlyExit(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE) return;

        GameEventsManager.instance.dialogueEvents.EarlyExitDialogue();
    }

    void ExitDialogue()
    {
        IsInDialogue = false;

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
