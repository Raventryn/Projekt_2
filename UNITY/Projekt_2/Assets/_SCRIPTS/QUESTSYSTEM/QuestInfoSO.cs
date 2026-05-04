using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName= "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id {get; private set;}

    [Header("General")]
    public string DisplayName;

    [Header("Requirements")]
    public QuestInfoSO[] QuestPrerequesites;

    [Header("Steps")]
    public GameObject[] QuestStepPrefabs;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
