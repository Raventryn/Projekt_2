using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "ScriptableObjects/PlayerSettingsSO", order = 4)]
public class PlayerSettingsSO : ScriptableObject
{
    [Range(2f, 15f)]
    public float LookSensitivity = 6f;
}
