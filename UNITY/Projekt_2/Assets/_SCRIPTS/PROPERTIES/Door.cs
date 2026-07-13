using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    [SerializeField] Vector3 _openRotation;
    Collider _doorCollider;
    Vector3 _closedRotation;
    Vector3 _originRotation;

    bool _isDoorOpen;
    bool _isObjectMoving;
    bool _isInteactionBlocked;

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onInteraction += OpenDoor;
    }
    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onInteraction -= OpenDoor;
    }

    void Start()
    {
        _closedRotation = transform.localEulerAngles;
        _doorCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (_isObjectMoving)
        {
            switch (_isDoorOpen)
            {
                case true:
                    RotateDoor(_closedRotation);
                    break;
                case false:
                    RotateDoor(_openRotation);
                    break;
            }
        }
    }

    void OpenDoor(GameObject gameObject)
    {
        if(gameObject != this.gameObject || _isInteactionBlocked ) return;


        _isObjectMoving = true;
        _isInteactionBlocked = true;

        _doorCollider.enabled = false;
    }

    void RotateDoor(Vector3 target)
    {

        float angleDifference = Quaternion.Angle(Quaternion.Euler(target), transform.rotation);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target), (5f + angleDifference * 2) * Time.deltaTime);
         
        switch (_isDoorOpen)
        {
            case true:
                if(angleDifference <= 0.01f)
                {
                    transform.localEulerAngles = target;
                    _isObjectMoving = false;
                    _isDoorOpen = false;

                    _doorCollider.enabled = true;

                    _isInteactionBlocked = false;
                }
                break;
            case false:
                if(angleDifference <= 0.01f)
                {
                    transform.localEulerAngles = target;
                    _isObjectMoving = false;
                    _isDoorOpen = true;

                    StartCoroutine(DelayDoorClosing());
                }
                break;
        }
    }

    IEnumerator DelayDoorClosing()
    {
        yield return new WaitForSeconds(1f);

        _isObjectMoving = true;
    }
}
