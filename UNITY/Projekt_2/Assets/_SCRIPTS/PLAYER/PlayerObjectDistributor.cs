using UnityEngine;

public class PlayerObjectDistributor : MonoBehaviour
{
    void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onRequestPlayer += SendPlayerObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onRequestPlayer -= SendPlayerObject;
    }

    void SendPlayerObject(GameObject callerObject)
    {
        GameEventsManager.instance.playerEvents.ReturnPlayer(gameObject, callerObject);
    }
}
