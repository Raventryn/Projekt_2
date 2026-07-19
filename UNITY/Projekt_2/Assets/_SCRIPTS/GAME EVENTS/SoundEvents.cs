using System;
using System.Runtime.CompilerServices;
using UnityEngine;
public class SoundEvents
{
    public event Action<SoundType, bool> onTriggerSound;

    public void TriggerSound(SoundType sound, bool isPersistent)
    {
        if(onTriggerSound != null)
        {
            onTriggerSound(sound, isPersistent);
        }
    }

    public event Action<AudioClip, bool> onSendAudioClip;

    public void SendAudioClip(AudioClip clip, bool isPersistent)
    {
        if(onSendAudioClip != null)
        {
            onSendAudioClip(clip, isPersistent);
        }
    }

    public event Action onStopSound;

    public void StopSound()
    {
        if(onStopSound != null)
        {
            onStopSound();
        }
    }
}

public enum SoundType
{
    UI_CLICK,
    UI_OPEN,

    WALK,
    DOOR_OPEN,
    DOOR_CLOSE,
    DOOR_LOCKED,
    SCAN_START,
    SCAN,
    SCAN_END,
    WAKE_UP,
    ITEM_PICKUP,
    GLITCH,
    DRAWER_OPEN,
    DRAWER_CLOSE,
    CLOSET_OPEN,
    CLOSET_CLOSE,

}