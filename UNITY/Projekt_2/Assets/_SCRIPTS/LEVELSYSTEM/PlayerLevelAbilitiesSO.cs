using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName= "PlayerLevelAbilitiesSO", menuName = "ScriptableObjects/PlayerLevelAbilities_SO", order = 2)]
public class PlayerLevelAbilitiesSO : ScriptableObject
{
    public int[] moneyRequirements = new int[6];

    public AbilityType[] types = new AbilityType[6];

    public float[] values = new float[6];
}
