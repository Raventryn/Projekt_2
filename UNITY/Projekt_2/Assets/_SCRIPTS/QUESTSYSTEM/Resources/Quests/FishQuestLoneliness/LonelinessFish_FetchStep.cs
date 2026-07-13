using UnityEngine;

public class LonelinessFish_FetchStep : QuestStep
{
    void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onAddItem += FinishStep;
    }

    void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onAddItem -= FinishStep;
    }

    void FinishStep(string id)
    {
        if(id != "INVENTORY:GUPPY") return;

        FinishQuestStep();
    }
}
