using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettingsSO", menuName = "ScriptableObjects/PlayerSettingsSO", order = 4)]
public class PlayerSettingsSO : ScriptableObject
{
    [Range(2f, 15f)]
    public float LookSensitivity = 6f;

    [Range(-80f, 0f)]
    public float MasterVolume;
    [Range(-80f, 0f)]
    public float VoicesVolume;
    [Range(-80f, 0f)]
    public float EffectsVolume;
    [Range(-80f, 0f)]
    public float EnvironmentVolume;

    public Resolution ScreenResolution;
    public FullScreenMode fullscreenMode;
}
