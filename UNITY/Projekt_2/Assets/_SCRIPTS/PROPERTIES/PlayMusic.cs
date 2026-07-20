using System.Collections;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    AudioSource audioSource;

    float maxVolume;

    bool isFadingIn;

    void OnEnable()
    {
        GameEventsManager.instance.soundEvents.onPlayMusic += PlayAudio;
    }

    void OnDisable()
    {
        GameEventsManager.instance.soundEvents.onPlayMusic -= PlayAudio;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
        audioSource.volume = 0;
    }

    void Update()
    {
        if (isFadingIn)
        {
            audioSource.volume += 0.1f * Time.deltaTime;

            if(audioSource.volume >= maxVolume)
            {
                audioSource.volume = maxVolume;
                isFadingIn = false;
                this.enabled = false;
            }
        }
    }

    void PlayAudio()
    {
        audioSource.Play();
        isFadingIn = true;
    }
}
