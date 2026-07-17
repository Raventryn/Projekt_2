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

    public event Action<bool, ScannableObjectType, ScanObject[]> onShowButtonCanvas;

    public void ShowButtonCanvas(bool toggle, ScannableObjectType type, ScanObject[] objects)
    {
        onShowButtonCanvas?.Invoke(toggle, type, objects);
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

    public event Action<bool,ScannableObjectType> onStartScanMinigame;

    public void StartScanMinigame(bool toggle, ScannableObjectType type)
    {
        onStartScanMinigame?.Invoke(toggle, type);
    }

    public event Action onFinishedScanMinigame;

    public void FinishedScanMinigame()
    {
        onFinishedScanMinigame?.Invoke();
    }

    public event Action<ScannableObjectType> onHideScanGlitch;

    public void HideScanGlitch(ScannableObjectType type)
    {
        onHideScanGlitch?.Invoke(type);
    }

    public event Action<bool> onStartFishMinigame;

    public void StartFishMinigame(bool toggle)
    {
        onStartFishMinigame?.Invoke(toggle);
    }

    public event Action onGrilledRoach;

    public void GrilledRoach()
    {
        onGrilledRoach?.Invoke();
    }

    public event Action<bool> onAllowRoachScan;

    public void AllowRoachScan(bool toggle)
    {
        onAllowRoachScan?.Invoke(toggle);
    }

    public event Action onStartCalibrationMinigame;

    public void StartCalibrationMinigame()
    {
        onStartCalibrationMinigame?.Invoke();
    }

    public event Action onFinishedCalibrationMinigame;

    public void FinishedCalibrationMinigame()
    {
        onFinishedCalibrationMinigame?.Invoke();
    }

    public event Action onSitPlayerUp;

    public void SitPlayerUp()
    {
        onSitPlayerUp?.Invoke();
    }

    public event Action onDisableGlitches;

    public void DisableGlitches()
    {
        onDisableGlitches?.Invoke();
    }
}
