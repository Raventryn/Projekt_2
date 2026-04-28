using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] bool _setRotationOnce;

    GameObject _playerObject;
    bool _rotationSet = false;

    void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onReturnPlayer += SetPlayerObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onReturnPlayer -= SetPlayerObject;
    }
    
    void Start()
    {
        GameEventsManager.instance.playerEvents.RequestPlayer(gameObject);
    }

    void Update()
    {
        if (_setRotationOnce)
        {
            if (!_rotationSet)
            {
                transform.LookAt(_playerObject.transform.position);
                _rotationSet = true;
            }
        }
        else
        {
            transform.LookAt(_playerObject.transform.position);
        }
        
        
    }

    void SetPlayerObject(GameObject playerObject, GameObject callerObject)
    {
        if(callerObject != this.gameObject) return;

        _playerObject = playerObject;
    }
}
