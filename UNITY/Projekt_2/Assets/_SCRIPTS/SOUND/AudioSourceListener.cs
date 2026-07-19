using System;
using UnityEngine;

public class AudioSourceListener : MonoBehaviour
{
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource persistentSource;

    private void OnEnable()
    {
        GameEventsManager.instance.soundEvents.onSendAudioClip += PlaySound;
        GameEventsManager.instance.soundEvents.onStopSound += StopSound;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.soundEvents.onSendAudioClip -= PlaySound;
        GameEventsManager.instance.soundEvents.onStopSound -= StopSound;
    }

    private void PlaySound(AudioClip clip, bool isPersistent)
    {
        Debug.Log("Received Play Audio event: " + clip);

        if (isPersistent)
        {
            persistentSource.generator = clip;
            persistentSource.Play();
        }
        else
        {
            oneShotSource.generator = clip;
            oneShotSource.PlayOneShot(clip);
        }
    }

    private void StopSound()
    {
        persistentSource.Stop();
        oneShotSource.Stop();
    }
}
