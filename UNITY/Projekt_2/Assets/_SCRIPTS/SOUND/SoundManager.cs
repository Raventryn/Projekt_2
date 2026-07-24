using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundLibrary library;

    private void Awake()
    {
        GameEventsManager.instance.soundEvents.onTriggerSound += SendAudioClip;
    }
    private void OnDestroy()
    {
        GameEventsManager.instance.soundEvents.onTriggerSound -= SendAudioClip;
    }


    private void SendAudioClip(SoundType sound, bool isPersistent)
    {
        Debug.Log("Received Send Audio event: " + sound);

        switch (sound)
        {
            case SoundType.UI_CLICK:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.UI_Click, isPersistent);
                break;
            case SoundType.UI_OPEN:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.UI_Open, isPersistent);
                break;
            case SoundType.DOOR_OPEN:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.DOOR_OPEN, isPersistent);
                break;
            case SoundType.DOOR_LOCKED:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.DOOR_LOCKED, isPersistent);
                break;
            case SoundType.DOOR_CLOSE:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.DOOR_CLOSE, isPersistent);
                break;
            case SoundType.SCAN_START:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.SCAN_START, isPersistent);
                break;
            case SoundType.SCAN_END:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.SCAN_END, isPersistent);
                break;
            case SoundType.SCAN:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.SCAN, isPersistent);
                break;    
            case SoundType.WAKE_UP:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.WAKE_UP, isPersistent);
                break;
            case SoundType.ITEM_PICKUP:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.ITEM_PICKUP, isPersistent);
                break;      
            case SoundType.GLITCH:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.GLITCH, isPersistent);
                break;   
            case SoundType.CLOSET_OPEN:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.CLOSET_OPEN, isPersistent);
                break;
            case SoundType.CLOSET_CLOSE:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.CLOSET_CLOSE, isPersistent);
                break;   
            case SoundType.DRAWER_OPEN:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.DRAWER_OPEN, isPersistent);
                break;  
            case SoundType.DRAWER_CLOSE:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.DRAWER_CLOSE, isPersistent);
                break;      
            case SoundType.SIZZLE:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.SIZZLE, isPersistent);
                break;
            case SoundType.MAIN_MENU_MUSIC:
                GameEventsManager.instance.soundEvents.SendAudioClip(library.MENU_MUSIC, isPersistent);
                break;
        }
    }
}
