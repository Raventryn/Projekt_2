using UnityEngine;

public class DummyQuestStep_One : QuestStep
{
    private int _objectsScanned = 0;

    private int _objectsToScan = 2;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onObjectScanned += ObjectScanned;
    }
    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onObjectScanned -= ObjectScanned;
    }

    void ObjectScanned()
    {
        if(_objectsScanned < _objectsToScan) _objectsScanned++;

        if(_objectsScanned >= _objectsToScan) FinishQuestStep();
    }
}
