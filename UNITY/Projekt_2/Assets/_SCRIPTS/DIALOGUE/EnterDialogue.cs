using UnityEngine;
using TMPro;
using UnityEngine.Animations;

public class EnterDialogue : MonoBehaviour
{
    [SerializeField] Camera _dialogueCamera;

    [SerializeField] TMP_Text _dialogueText;

    Vector3 _dCameraPosition;

    Vector3 _mCameraPosition;

    LookAtConstraint _cameraLookAt;

    ConstraintSource _constraintSource;

    public float CameraTransitionSpeed;

    void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onInteraction += TransitionCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape += ExitCamera;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onInteraction -= TransitionCamera;
        GameEventsManager.instance.inputEvents.onPressedEscape -= ExitCamera;
    }

    void TransitionCamera(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;

        Debug.Log("Entered");
        
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(false);
        GameEventsManager.instance.playerEvents.LockPlayerCamera(false);

        _cameraLookAt = Camera.main.GetComponent<LookAtConstraint>();

        _mCameraPosition = Camera.main.transform.position;

        _dCameraPosition = _dialogueCamera.transform.position;

        _constraintSource = _cameraLookAt.GetSource(0);
        _constraintSource.sourceTransform = gameObject.transform;

        MoveInCamera();
    }

    void MoveInCamera()
    {
        Vector3 direction = _dCameraPosition - Camera.main.transform.position;

        bool cameraInPosition = false;

        _cameraLookAt.constraintActive = true;

        while(!cameraInPosition)
        {
            Camera.main.transform.Translate(direction.normalized * CameraTransitionSpeed * Time.deltaTime);

            if(direction.magnitude >= 0.1f)
            {
                cameraInPosition = true;
            }
        }

        _dialogueText.enabled = true;
    }

    void MoveOutCamera()
    {
        Vector3 direction = - Camera.main.transform.position - _mCameraPosition ;

        bool cameraInPosition = false;

        _dialogueText.enabled = false;

        while(!cameraInPosition)
        {
            Camera.main.transform.Translate(direction.normalized * CameraTransitionSpeed * Time.deltaTime);

            if(direction.magnitude >= 0.1f)
            {
                cameraInPosition = true;
            }
        }

        _cameraLookAt.constraintActive = false;
    }

    void ExitCamera(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE) return;
    
        MoveOutCamera();

        _constraintSource.sourceTransform = null;

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DEFAULT);
        GameEventsManager.instance.playerEvents.LockPlayerMovement(true);
        GameEventsManager.instance.playerEvents.LockPlayerCamera(true);
    }
}
