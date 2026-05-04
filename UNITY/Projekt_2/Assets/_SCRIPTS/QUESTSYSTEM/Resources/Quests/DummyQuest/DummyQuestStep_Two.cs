using UnityEngine;

public class DummyQuestStep_Two : QuestStep
{
    private int _objectsInterpreted = 0;

    private int Interpret = 1;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onInterpretedObject += ObjectInterpreted;
    }
    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onInterpretedObject -= ObjectInterpreted;
    }

    void ObjectInterpreted()
    {
        if(_objectsInterpreted < Interpret) _objectsInterpreted++;

        if(_objectsInterpreted >= Interpret) FinishQuestStep();
    }
}
