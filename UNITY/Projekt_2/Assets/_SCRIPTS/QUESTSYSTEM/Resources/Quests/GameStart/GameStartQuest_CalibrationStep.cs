using UnityEngine;

public class GameStartQuest_CalibrationStep : QuestStep
{
    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onFinishedCalibrationMinigame += MinigameFinished;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onFinishedCalibrationMinigame -= MinigameFinished;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameEventsManager.instance.questEvents.StartCalibrationMinigame();
    }

    void MinigameFinished()
    {
        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.DIALOGUE);
    }
}
