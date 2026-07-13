using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] Animator _cameraAnimator;
    [SerializeField] CinemachineCamera _animationCamera;
    [SerializeField] List<AnimationClip> _animationClips = new List<AnimationClip>();

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onSitPlayerUp += PlayLastAnimation;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onSitPlayerUp -= PlayLastAnimation;
    }

    void Start()
    {
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(false);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(false);

        GameEventsManager.instance.playerEvents.BlockPlayerInput(true);

        StartCoroutine(AwaitAnimation(1));
        _animationCamera.Priority = 1;
    }

    void ReleasePlayer()
    {
        _animationCamera.Priority = -1;
        GameEventsManager.instance.playerEvents.TogglePlayerCamera(true);
        GameEventsManager.instance.playerEvents.TogglePlayerMovement(true);
        GameEventsManager.instance.playerEvents.BlockPlayerInput(false);
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
                yield return new WaitForSeconds(0.5f);

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
                break;
        }
        
    }
}
