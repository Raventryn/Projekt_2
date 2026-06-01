using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName= "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {get; private set;}

    [Header("General")]
    public string DisplayName;

    [Header("Requirements")][Tooltip("SO of Quests required to start this quest")]
    public QuestInfoSO[] QuestPrerequesites;

    [Header("Steps")][Tooltip("GO Prefabs of steps in this quest")]
    public GameObject[] QuestStepPrefabs;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
