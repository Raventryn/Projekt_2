using System.Collections.Generic;
using UnityEngine;

public class RoachQuest_Step_1 : QuestStep
{
    int grilledRoaches;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onGrilledRoach += CountGrilledRoaches;
    }
    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onGrilledRoach -= CountGrilledRoaches;
    }

    void Start()
    {
        GameEventsManager.instance.questEvents.AllowRoachScan(true);
    }

    void CountGrilledRoaches()
    {
        grilledRoaches++;
        Debug.Log(grilledRoaches);

        if(grilledRoaches >= 10)
        {
            GameEventsManager.instance.questEvents.AllowRoachScan(false);
            FinishQuestStep();
        }
    }
}
