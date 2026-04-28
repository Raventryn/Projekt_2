using System;
using UnityEngine;

public class PlayerEvents
{
    public event Action<bool> onLockPlayerMovement;

    public void LockPlayerMovement(bool toggle)
    {
        if(onLockPlayerMovement != null)
        {
            onLockPlayerMovement(toggle);
        }
    }

    public event Action<bool> onLockPlayerCamera;

    public void LockPlayerCamera(bool toggle)
    {
        if(onLockPlayerCamera != null)
        {
            onLockPlayerCamera(toggle);
        }
    }

    public event Action<bool> onShowPlayerCharacter;

    public void ShowPlayerCharacter(bool toggle)
    {
        onShowPlayerCharacter?.Invoke(toggle);
    }

    public event Action<GameObject> onRequestPlayer;

    public void RequestPlayer(GameObject callerObject)
    {
        onRequestPlayer?.Invoke(callerObject);
    }

    public event Action<GameObject, GameObject> onReturnPlayer;

    public void ReturnPlayer(GameObject playerObject, GameObject callerObject)
    {
        onReturnPlayer?.Invoke(playerObject, callerObject);
    }
}
