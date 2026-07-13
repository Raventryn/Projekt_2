using UnityEngine;

public class GameStart_FinalStep : QuestStep
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameEventsManager.instance.questEvents.SitPlayerUp();

        FinishQuestStep();
    }


}
