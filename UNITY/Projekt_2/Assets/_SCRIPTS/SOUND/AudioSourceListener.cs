using System;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioSourceListener : MonoBehaviour
{
    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private AudioSource persistentSource;

    float volumeTarget;
    bool isChangingVolume;

    private void OnEnable()
    {
        GameEventsManager.instance.soundEvents.onSendAudioClip += PlaySound;
        GameEventsManager.instance.soundEvents.onStopSound += StopSound;
        GameEventsManager.instance.soundEvents.onChangeAudioVolume += ChangeVolume;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.soundEvents.onSendAudioClip -= PlaySound;
        GameEventsManager.instance.soundEvents.onStopSound -= StopSound;
        GameEventsManager.instance.soundEvents.onChangeAudioVolume -= ChangeVolume;
    }

    void Update()
    {
        if (isChangingVolume)
        {
            oneShotSource.volume = Mathf.MoveTowards(oneShotSource.volume, volumeTarget, 0.2f);
            persistentSource.volume = Mathf.MoveTowards(persistentSource.volume, volumeTarget, 0.2f);

            if(persistentSource.volume == volumeTarget)
            {
                isChangingVolume = false;
            }
        }
    }

    private void PlaySound(AudioClip clip, bool isPersistent)
    {
        //Debug.Log("Received Play Audio event: " + clip);

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

    private void ChangeVolume(float value)
    {
        volumeTarget = value;
        isChangingVolume = true;
    }
}
