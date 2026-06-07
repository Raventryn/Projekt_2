using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObjects/DialogueAudioInfoSO", order = 3)]
public class DialogueAudioInfoSO : ScriptableObject
{
    public string id;

    public AudioClip[] dialogueTypingSoundClips;
    [Range(1, 5)]
    public int frequencyLevel = 2;
    [Range(0.5f, 3)]
    public float minPitch;
    [Range(0.5f, 3)]
    public float maxPitch;
    public bool stopAudioSource;
}
