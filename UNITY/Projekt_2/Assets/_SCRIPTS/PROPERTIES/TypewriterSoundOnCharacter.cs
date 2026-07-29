using Febucci.TextAnimatorForUnity.TextMeshPro;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorCore.Text;
using UnityEngine;
using Febucci.TextAnimatorCore;

public class TypewriterSoundOnCharacter : MonoBehaviour
{
    [SerializeField] TypewriterComponent typewriter;
    [SerializeField] TextAnimator textAnimator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    [SerializeField] bool stopAudioSource;

    int _visibleCharacters;

    int frequencyLevel = 3;

    void OnEnable()
    {
        typewriter.onCharacterVisible.AddListener(PlayTypewriterSound);
    }

    void OnDisable()
    {
        typewriter.onCharacterVisible.RemoveListener(PlayTypewriterSound);
    }

    void PlayTypewriterSound(CharacterData charData)
    {
        _visibleCharacters++;

        if(_visibleCharacters % frequencyLevel == 0)
        {
            if (stopAudioSource)
            {
                audioSource.volume = 0;
                audioSource.Stop();
                audioSource.volume = 1;
            }

            audioSource.pitch = Random.Range(0.95f, 1.05f);

            audioSource.PlayOneShot(audioClip);
        }
    }
}
