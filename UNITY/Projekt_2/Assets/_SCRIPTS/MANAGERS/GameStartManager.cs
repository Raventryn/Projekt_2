using System;
using System.Collections;
using System.Collections.Generic;
using KinoGlitch;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] Animator _cameraAnimator;
    [SerializeField] CinemachineCamera _animationCamera;
    [SerializeField] List<AnimationClip> _animationClips = new List<AnimationClip>();
    [SerializeField] DigitalGlitchController _glitchController;
    [SerializeField] float _glitchFadeTime;

    float _glitchThreshold;
    bool isDecreaseGlitch;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onSitPlayerUp += PlayLastAnimation;
        GameEventsManager.instance.questEvents.onDisableGlitches += DisableGlitch;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onSitPlayerUp -= PlayLastAnimation;
        GameEventsManager.instance.questEvents.onDisableGlitches -= DisableGlitch;
    }

    void Start()
    {
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

        GameEventsManager.instance.playerEvents.BlockPlayerInput(true);

        StartCoroutine(AwaitAnimation(1));
        _animationCamera.Priority = 1;

        _glitchController.Intensity = 1;
        _glitchThreshold = 0.08f;

        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.WAKE_UP, false);
        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.STARTGLITCHES, true);
    }

    void Update()
    {
        if(_glitchController.Intensity > _glitchThreshold && isDecreaseGlitch)
        {
            DecreaseGlitchStrength();
        }
    }

    void DecreaseGlitchStrength()
    {
        _glitchController.Intensity -= (1 / _glitchFadeTime) * Time.deltaTime;

        if (_glitchController.Intensity <= 0)
        {
            _glitchController.Intensity = 0;
            isDecreaseGlitch = false;
        }
    }

    void DisableGlitch()
    {
        _glitchThreshold = 0;
        GameEventsManager.instance.soundEvents.StopSound();
        //Disable Particles
    }

    void ReleasePlayer()
    {
        _animationCamera.Priority = -1;
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.BlockPlayerInput(false);

        GameEventsManager.instance.uiEvents.ShowScannerTooltip(true);
        GameEventsManager.instance.soundEvents.PlayMusic();
    }

    void TestNextCoroutine(int stage)
    {
        StartCoroutine(AwaitAnimation(stage));
    }

    void PlayLastAnimation()
    {
        StartCoroutine(AwaitAnimation(3));
    }

    IEnumerator AwaitAnimation(int stage)
    {
        Debug.Log("Starting Animation");
        switch (stage)
        {
            case 1:

                yield return new WaitForSeconds(1);

                isDecreaseGlitch = true;

                yield return new WaitForSeconds(1.5f);

                _cameraAnimator.SetTrigger("Awake");

                yield return new WaitForSeconds(_animationClips[0].length);
                Debug.Log("Ended Animation");
                yield return new WaitForSeconds(1f);
                TestNextCoroutine(2);

                break;

            case 2:
                _cameraAnimator.SetTrigger("Sit");

                yield return new WaitForSeconds(_animationClips[1].length);
                Debug.Log("Ended Animation");

                GameEventsManager.instance.playerEvents.BlockPlayerInput(false);

                break;

            case 3:
                _cameraAnimator.SetTrigger("Stand");

                yield return new WaitForSeconds(_animationClips[2].length - 0.05f);
                Debug.Log("Ended Animation");
                ReleasePlayer();
                this.enabled = false;
                break;
        }
        
    }
}
