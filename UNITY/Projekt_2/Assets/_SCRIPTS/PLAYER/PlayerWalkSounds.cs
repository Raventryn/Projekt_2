using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class PlayerWalkSounds : MonoBehaviour
{
    [SerializeField] Player_Controller playerController;
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    AudioSource audioSource;

    bool isInvoking;

    int lastRandomIndex;

    void OnEnable()
    {
        GameEventsManager.instance.soundEvents.onPlayStepSounds += StartInvoke;
        GameEventsManager.instance.soundEvents.onStopStepSounds += StopInvoke;
    }

    void OnDisable()
    {
        GameEventsManager.instance.soundEvents.onPlayStepSounds -= StartInvoke;
        GameEventsManager.instance.soundEvents.onStopStepSounds -= StopInvoke;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void StartInvoke()
    {
        StartCoroutine(WaitBetweenSteps());
    }

    void StopInvoke()
    {
        StopAllCoroutines();
    }

    void PlayStepSound()
    {
        audioSource.PlayOneShot(SelectRandomClip());
    }

    void GenerateRandomIndex()
    {
        int randomIndex = Random.Range(0, audioClips.Count);

        if(randomIndex == lastRandomIndex)
        {
            GenerateRandomIndex();
        }
        else
        {
            lastRandomIndex = randomIndex;
        }
    }

    AudioClip SelectRandomClip()
    {
        GenerateRandomIndex();

        return audioClips[lastRandomIndex];
    }

    IEnumerator WaitBetweenSteps()
    {
        PlayStepSound();

        yield return new WaitForSeconds(Mathf.Clamp(1 / playerController.MoveSpeed, 0.3f, 0.55f));

        StartCoroutine(WaitBetweenSteps());
    }
}
