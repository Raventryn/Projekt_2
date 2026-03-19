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
}
