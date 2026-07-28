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

    [SerializeField] List<CinemachineCamera> _dialogueCamera = new List<CinemachineCamera>();

    [SerializeField] GameObject _contentParent;

    [SerializeField] TextMeshProUGUI _dialogueText;
    [SerializeField] TextAnimator_TMP dialogueAnimator;
    [SerializeField] TypewriterComponent _dialogueTypewriter;

    [SerializeField] DialogueChoiceButton[] _choiceButtons;

    [SerializeField] DialogueAudioInfoSO _dialogueAudioInfo;

    Vector3 _cameraDefaultPosition;
    float _offsetClampValue;
    bool IsInDialogue;
    bool IsOffsetCamera;
    bool IsCurrentDialogueObject;

    int currentCameraIndex;

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

        //GameEventsManager.instance.dialogueEvents.onDialogueFinished += SendFinishedEvent;
        GameEventsManager.instance.dialogueEvents.onAdvanceDialogueCamera += AdvanceCurrentCamera;
    }

    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onDialogueInteraction -= EnterCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape -= EarlyExit;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= ExitDialogue;

        //GameEventsManager.instance.dialogueEvents.onDialogueFinished -= SendFinishedEvent;
        GameEventsManager.instance.dialogueEvents.onAdvanceDialogueCamera -= AdvanceCurrentCamera;
    }

    void Update()
    {
        if (IsInDialogue && VirtualMouseCursor.instance.IsCursorVisible && IsOffsetCamera)
        {
            OffsetScanViewCamera();
        }

        if (IsInDialogue)
        {
            _contentParent.transform.position = SetCanvasWorldPosition();
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

        GameEventsManager.instance.soundEvents.ChangeAudioVolume(0.25f);

        GameEventsManager.instance.dialogueEvents.PassDialogueUIPanel(_contentParent, _dialogueTypewriter, dialogueAnimator, _choiceButtons, _dialogueAudioInfo);

        _cameraDefaultPosition = _dialogueCamera[0].transform.localPosition;

        _dialogueCamera[0].Priority = 1;

        currentCameraIndex = 0;

        _offsetClampValue = (this.gameObject.transform.position - _dialogueCamera[0].transform.position).magnitude;
        //Debug.Log(_offsetClampValue);

        IsInDialogue = true;
        IsOffsetCamera = true;
        IsCurrentDialogueObject = true;

        SendDialogueEvent();
    }

    void OffsetScanViewCamera()
    {
        Vector2 screenUV = new Vector2((VirtualMouseCursor.instance.CursorScreenPosition.x / Screen.width) -0.5f, (VirtualMouseCursor.instance.CursorScreenPosition.y / Screen.height) -0.5f);

        //Debug.Log(screenUV);

        _dialogueCamera[currentCameraIndex].transform.Translate(_dialogueCamera[currentCameraIndex].transform.right * VirtualMouseCursor.instance.ClampedDelta.x * Time.deltaTime * -0.001f * _offsetClampValue);
        _dialogueCamera[currentCameraIndex].transform.Translate(_dialogueCamera[currentCameraIndex].transform.up * VirtualMouseCursor.instance.ClampedDelta.y * Time.deltaTime * 0.001f * _offsetClampValue);

        //float xPos = _cameraDefaultPosition.z + (screenUV.x / (5f * _offsetClampValue)); 
        //float yPos = _cameraDefaultPosition.y + (screenUV.y / (5f * _offsetClampValue));//Mathf.Clamp(_camera.transform.localPosition.z + (screenUV.y / 5), _cameraDefaultPosition.z - 0.1f, _cameraDefaultPosition.z + 0.1f);

        //Vector3 newPosition = new Vector3(_cameraDefaultPosition.x, yPos, xPos);

        //_dialogueCamera[currentCameraIndex].transform.localPosition = Vector3.MoveTowards(_dialogueCamera[currentCameraIndex].transform.localPosition, newPosition, 0.1f + Time.deltaTime);
    }

    void AdvanceCurrentCamera(string cameraIndexString)
    {
        if(!IsCurrentDialogueObject) return;

        IsOffsetCamera = false;

        int cameraIndex = int.Parse(cameraIndexString);
        _dialogueCamera[cameraIndex].Priority = 2;
        _dialogueCamera[currentCameraIndex].Priority = -1;
        
        _cameraDefaultPosition = _dialogueCamera[cameraIndex].transform.localPosition;

        currentCameraIndex = cameraIndex;

        _offsetClampValue = (this.gameObject.transform.position - _dialogueCamera[currentCameraIndex].transform.position).magnitude;

        _dialogueCamera[cameraIndex].Priority = 1;

        IsOffsetCamera = true;
    }

    void EarlyExit(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE) return;

        GameEventsManager.instance.dialogueEvents.EarlyExitDialogue();
    }

    void ExitDialogue()
    {
        if(!IsInDialogue) return;

        IsInDialogue = false;

        IsCurrentDialogueObject = false;

        _dialogueCamera[currentCameraIndex].Priority = -1;

        _dialogueCamera[currentCameraIndex].LookAt = null;

        GameEventsManager.instance.dialogueEvents.ClearDialogueUIPanel();

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.ShowPlayerCharacter(true);

        GameEventsManager.instance.inputEvents.ShowCursor(false);

        GameEventsManager.instance.soundEvents.ChangeAudioVolume(1f);
    }

    private void ResetPanel()
    {
        _dialogueText.text = "";
    }

    public Vector3 SetCanvasWorldPosition()
    {
        Vector2 viewportPoint = Camera.main.ViewportToScreenPoint(new Vector2(0.3f, 0.5f));

        Vector3 newPoint = Camera.main.ScreenToWorldPoint(new Vector3(viewportPoint.x, viewportPoint.y, 1f));

        return newPoint;
    }
}
