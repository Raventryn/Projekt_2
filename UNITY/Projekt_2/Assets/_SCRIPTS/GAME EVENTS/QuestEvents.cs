using System;
using UnityEngine;

public class QuestEvents
{
    //Generic quest events for questsystem
    public event Action<string> onStartQuest;

    public void StartQuest(string id)
    {
        onStartQuest?.Invoke(id);
    }

    public event Action<string> onAdvanceQuest;

    public void AdvanceQuest(string id)
    {
        onAdvanceQuest?.Invoke(id);
    }

    public event Action<string> onFinishQuest;

    public void FinishQuest(string id)
    {
        onFinishQuest?.Invoke(id);
    }

    public event Action<Quest> onQuestStateChange;

    public void QuestStateChange(Quest quest)
    {
        onQuestStateChange?.Invoke(quest);
    }


    //Below here are Specific quest events 
    public event Action<string> onChangeSphereColour;

    public void ChangeSphereColour(string colour)
    {
        onChangeSphereColour?.Invoke(colour);
    }

    public event Action<bool, ScannableObjectType> onShowButtonCanvas;

    public void ShowButtonCanvas(bool toggle, ScannableObjectType type)
    {
        onShowButtonCanvas?.Invoke(toggle, type);
    }

    public event Action<ScannableObjectType, GameObject> onReplaceInterpretableObjects;

    public void ReplaceInterpretableObjects(ScannableObjectType type, GameObject replacerObject)
    {
        onReplaceInterpretableObjects?.Invoke(type, replacerObject);
    }

    public event Action onObjectScanned;

    public void ObjectScanned()
    {
        onObjectScanned ?.Invoke();
    }

    public event Action onInterpretedObject;

    public void ObjectInterpreted()
    {
        onInterpretedObject?.Invoke();
    }

    public event Action<bool> onStartScanMinigame;

    public void StartScanMinigame(bool toggle)
    {
        onStartScanMinigame?.Invoke(toggle);
    }
}
