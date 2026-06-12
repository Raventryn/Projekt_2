using System.Collections;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using Febucci.TextAnimatorCore.Text;
using Unity.Cinemachine;
using UnityEngine;

public class KukasTalkBehaviour : MonoBehaviour
{
    [SerializeField] KukasVoiceLinesSO voiceLinesSO;
    [SerializeField] TypewriterComponent typewriter;
    [SerializeField] TextAnimator_TMP textAnimator;
    [SerializeField] Canvas canvas;
    [SerializeField] Animator canvasAnimator;

    [Tooltip("Time in seconds between voicelines")]    
    [SerializeField] float voiceLineFrequency = 10;

    [SerializeField] private bool makePredictable;

    [SerializeField] DialogueAudioInfoSO currentAudioInfo;
    [SerializeField] private AudioSource audioSource;

    int _visibleCharacters;

    string _currentVoiceLine;
    int _currentVoiceIndex;
    int _previousVoiceIndex;

    void OnEnable()
    {
        typewriter.onTextShowed.AddListener(UnlockWaitTimer);
        typewriter.onCharacterVisible.AddListener(PlayTypewriterSound);
    }

    void OnDisable()
    {
        typewriter.onTextShowed.RemoveListener(UnlockWaitTimer);
        typewriter.onCharacterVisible.RemoveListener(PlayTypewriterSound);
    }

    void Start()
    {
        SelectVoiceLine();
    }

    void SelectVoiceLine()
    {
        bool isNewIndex = false;

        while (!isNewIndex)
        {
            int randomIndex = Random.Range(0, voiceLinesSO.VoiceLines.Count);

            if(randomIndex != _previousVoiceIndex)
                _currentVoiceIndex = randomIndex;
                isNewIndex = true;
        }
        


        _currentVoiceLine = voiceLinesSO.VoiceLines[_currentVoiceIndex];

        _previousVoiceIndex = _currentVoiceIndex;

        ShowOrHideCanvas(true);

        PrintVoiceLine();
    }

    void PrintVoiceLine()
    {
        typewriter.ShowText(_currentVoiceLine);
    }

    void UnlockWaitTimer()
    {
        StartCoroutine(VoiceLineTimings());
    }

    void ShowOrHideCanvas(bool toggle)
    {
        canvasAnimator.SetBool("isShowCanvas", toggle);
    }

    IEnumerator VoiceLineTimings()
    {
        yield return new WaitForSeconds(_currentVoiceLine.ToCharArray().Length / 10);

        ShowOrHideCanvas(false);

        yield return new WaitForSeconds(0.15f);

        typewriter.ShowText("");

        yield return new WaitForSeconds(voiceLineFrequency);

        SelectVoiceLine();
    }

    void PlayTypewriterSound(CharacterData charData)
    {
        _visibleCharacters++;

        if(_visibleCharacters == textAnimator.CharactersCount)
        {
            _visibleCharacters = 0;
            return;
        }

        char character = charData.info.character;

        AudioClip[] dialogueTypingSoundClips = currentAudioInfo.dialogueTypingSoundClips;
        int frequencyLevel = currentAudioInfo.frequencyLevel;
        float minPitch = currentAudioInfo.minPitch;
        float maxPitch = currentAudioInfo.maxPitch;
        bool stopAudioSource = currentAudioInfo.stopAudioSource;

        //Debug.Log(_visibleCharacters);

        if(_visibleCharacters % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.volume = 0;
                audioSource.Stop();
                audioSource.volume = 1;
            } 

            AudioClip soundClip = null;

            if(makePredictable)
            {
                int hashCode = character.GetHashCode();

                int predictableIndex = hashCode % dialogueTypingSoundClips.Length;
                soundClip = dialogueTypingSoundClips[predictableIndex];

                int minPitchInt = (int) (minPitch * 100);
                int maxPitchInt = (int) (maxPitch * 100);
                int pitchRangeInt = maxPitchInt - minPitchInt;

                if(pitchRangeInt != 0)
                {
                    int predictablePitchInt = (hashCode % pitchRangeInt) + minPitchInt;
                    float predictablePitch = predictablePitchInt / 100f;
                    audioSource.pitch = predictablePitch;
                }
                else
                {
                    audioSource.pitch = minPitch;
                }
            }
            else
            {
                int randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
                soundClip = dialogueTypingSoundClips[randomIndex];
                audioSource.pitch = Random.Range(minPitch, maxPitch);
            }
            Debug.Log(character);
            audioSource.PlayOneShot(soundClip);
        }
    }
}
