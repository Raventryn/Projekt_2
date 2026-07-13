using UnityEngine;

public class GameStart_ReceiveDataStep : QuestStep
{
    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onFinishedScanMinigame += FinishedMinigame;
    }
    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onFinishedScanMinigame -= FinishedMinigame;
    }

    void Start()
    {
        GameEventsManager.instance.questEvents.StartScanMinigame(true, ScannableObjectType.GAMESTART_DUMMY);
    }

    void FinishedMinigame()
    {
        GameEventsManager.instance.uiEvents.ShowInteractionWidget(InteractionType.DIALOGUE);
        FinishQuestStep();
    }

}
