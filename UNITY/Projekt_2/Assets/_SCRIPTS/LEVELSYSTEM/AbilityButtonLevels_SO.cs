using UnityEngine;

[CreateAssetMenu(fileName= "AbilityButtonLevelsSO", menuName = "ScriptableObjects/AbilityButtonLevels_SO", order = 2)]
public class AbilityButtonLevels_SO : ScriptableObject
{
    public int MoneyRequirement;

    public AbilityButtonLevels_SO LevelRequirement;

    public bool IsLevelUnlocked;
}
