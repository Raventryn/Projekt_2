using UnityEngine;

public class MoveOrRotateObject : MonoBehaviour
{
    public FurnitureType type;
    public OpenDirection openDirection;
    public bool IsObjectMoving;
    public Vector3 closedPosition;
    public Vector3 closedRotation;
    
    Vector3 _originRotation;
    Vector3 _originPosition;
    Vector3 _targetPosition;

    bool _IsDoorOpen;
    

    void Start()
    {
        closedPosition = transform.localPosition;
        closedRotation = transform.localEulerAngles;
    }

    void Update()
    {
        if (IsObjectMoving)
        {
            MoveObject(_targetPosition);
        }
    }

    public void SetObjectTarget(Vector3 target)
    {
        _targetPosition = target;

        _originPosition = transform.localPosition;

        _originRotation = transform.localEulerAngles;

        IsObjectMoving = true;

        _IsDoorOpen = !_IsDoorOpen;
    }

    void MoveObject(Vector3 targetPosition)
    {

        switch (type)
        {
            case FurnitureType.DRAWER:
                Vector3 directionP = targetPosition - _originPosition;
                transform.Translate(directionP * 2f * Time.deltaTime);

                if((targetPosition - transform.localPosition).magnitude <= 0.01f)
                {
                    transform.localPosition = targetPosition;
                    IsObjectMoving = false;
                }
                break;
            case FurnitureType.DOOR:
                Vector3 directionR = targetPosition - _originRotation;
                transform.Rotate(directionR * 5f * Time.deltaTime);

                switch (_IsDoorOpen)
                {
                    case true:
                        if(Mathf.DeltaAngle(transform.localEulerAngles.y, targetPosition.y) >= -0.01f)
                        {
                            transform.localEulerAngles = targetPosition;
                            IsObjectMoving = false;
                        }
                        break;
                    case false:
                        if(Mathf.DeltaAngle(transform.localEulerAngles.y, targetPosition.y) <= 0.01f)
                        {
                            transform.localEulerAngles = targetPosition;
                            IsObjectMoving = false;
                        }
                        break;
                }
                break;
        }
        
    }
}
