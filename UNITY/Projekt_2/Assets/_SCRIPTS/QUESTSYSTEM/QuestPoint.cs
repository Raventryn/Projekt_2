using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] QuestInfoSO QuestInfoForPoint;

    [Header("Config")]
    [SerializeField] bool startPoint;
    [SerializeField] bool finishPoint;

    private string questId;

    private QuestState currentQuestState;

    private void Awake()
    {
        questId = QuestInfoForPoint.id;
    }

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.interactionEvents.onInteraction += Interact;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.interactionEvents.onInteraction -= Interact;
    }

    void Interact(GameObject gameObject)
    {
        if(gameObject != this.gameObject) return;


        //Possibly remove if quests are triggered by dialogue
        /*if(currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }*/
    }

    private void QuestStateChange(Quest quest)
    {
        //only update quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with id: " + questId + "updated to state: " + currentQuestState);
        }
    }
}
