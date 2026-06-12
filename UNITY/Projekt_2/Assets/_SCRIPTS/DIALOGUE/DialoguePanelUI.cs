using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Febucci.TextAnimatorForUnity.TextMeshPro; // <- import Text Animator's namespace
using System.Collections;
using Ink.Runtime;
using Ink.Parsed;
using UnityEngine.UI;
using Unity.VisualScripting;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorCore;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorCore.Text;

public class DialoguePanelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private TypewriterComponent dialogueTypewriter;
    [SerializeField] private TextAnimator_TMP dialogueAnimator;
    [SerializeField] private DialogueChoiceButton[] choiceButtons;
    [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    [SerializeField] private bool makePredictable;

    private DialogueAudioInfoSO currentAudioInfo;
    [SerializeField] private AudioSource audioSource;

    private string _dialogueLine;

    int _visibleCharacters = 0;

    void Awake()
    {
        currentAudioInfo = defaultAudioInfo;
    }


    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted += DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished += DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue += DisplayDialogue;

        GameEventsManager.instance.dialogueEvents.onPassDialogueUIPanel += SetReferences;
        GameEventsManager.instance.dialogueEvents.onClearDialogueUIPanel += ClearReferences;

        GameEventsManager.instance.inputEvents.onPressedInteract += SkipDialogue;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onDialogueStarted -= DialogueStarted;
        GameEventsManager.instance.dialogueEvents.onDialogueFinished -= DialogueFinished;
        GameEventsManager.instance.dialogueEvents.onDisplayDialogue -= DisplayDialogue;

        GameEventsManager.instance.dialogueEvents.onPassDialogueUIPanel -= SetReferences;
        GameEventsManager.instance.dialogueEvents.onClearDialogueUIPanel -= ClearReferences;

        GameEventsManager.instance.inputEvents.onPressedInteract -= SkipDialogue;
    }

    private void SetReferences(GameObject contentParent, TypewriterComponent dialogueTypewriter, TextAnimator_TMP dialogueAnimator, DialogueChoiceButton[] choiceButtons, DialogueAudioInfoSO dialogueAudioInfo)
    {
        this.contentParent = contentParent;
        this.dialogueTypewriter = dialogueTypewriter;
        this.dialogueAnimator = dialogueAnimator;
        this.choiceButtons = choiceButtons;

        if(dialogueAudioInfo != null)
        {
            currentAudioInfo = dialogueAudioInfo;
        }

        dialogueTypewriter.onCharacterVisible.AddListener(PlayTypewriterSound);
    }

    private void ClearReferences()
    {
        dialogueTypewriter.onCharacterVisible.RemoveListener(PlayTypewriterSound);

        contentParent = null;
        dialogueTypewriter = null;
        dialogueAnimator = null;
        choiceButtons = null;
        currentAudioInfo = defaultAudioInfo;
    }

    private void DialogueStarted()
    {
        contentParent.SetActive(true);
        //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.UI_OPEN);
    }

    private void DialogueFinished()
    {
        contentParent.SetActive(false);
        //GameEventsManager.instance.soundEvents.TriggerSound(SoundType.UI_OPEN);
    }

    private void DisplayDialogue(string dialogueLine, List<Ink.Runtime.Choice> dialogueChoices)
    {
        _dialogueLine = dialogueLine;

        dialogueTypewriter.ShowText(dialogueLine);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE_PLAYING);

        if(dialogueLine.Contains("<br>"))
        {
            dialogueTypewriter.SkipTypewriter();
            GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
        }

        if(dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("More Dialogue Choices ("
                + dialogueChoices.Count + ") came through than are supported ("
                + choiceButtons.Length + ")");
        }

        foreach(DialogueChoiceButton choiceButton in choiceButtons) 
        {
            choiceButton.gameObject.SetActive(false);
        }

        int choiceButtonIndex = dialogueChoices.Count - 1;
        for(int inkChoiceIndex = 0;  inkChoiceIndex < dialogueChoices.Count; inkChoiceIndex++)
        {
            Ink.Runtime.Choice dialogueChoice = dialogueChoices[inkChoiceIndex];
            DialogueChoiceButton choiceButton = choiceButtons[choiceButtonIndex];

            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(dialogueChoice.text);
            choiceButton.SetChoiceIndex(inkChoiceIndex);

            choiceButtonIndex--;
        }

        GameEventsManager.instance.inputEvents.ShowCursor(true);
    }
    

    private void SkipDialogue(InputEventContext context)
    {
        if(context != InputEventContext.DIALOGUE_PLAYING) return;

        if(!dialogueTypewriter.IsShowingText) return;

        dialogueTypewriter.StopShowingText();

        dialogueAnimator.SetText(_dialogueLine);

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
    }

    public void StopTypewriter()
    {
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
    }

    void PlayTypewriterSound(CharacterData charData)
    {
        _visibleCharacters++;

        if(_visibleCharacters == dialogueAnimator.CharactersCount)
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


    /*private IEnumerator ShowChars(string bufferText)
    {
        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE_PLAYING);

        _dialoguePlaying = true;

        string dialogueLine = "";

        foreach(char character in bufferText)
        {
            dialogueLine += character;
            yield return new WaitForSeconds(0.03f);
            dialogueText.text = dialogueLine;
        }

        _dialoguePlaying = false;

        GameEventsManager.instance.inputEvents.ChangeInputContext(InputEventContext.DIALOGUE);
    }*/
}
