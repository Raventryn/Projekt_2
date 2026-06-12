using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "so_KukasVoiceLines", menuName ="ScriptableObjects/KukasVoiceLinesSO", order = 5)]
public class KukasVoiceLinesSO : ScriptableObject
{
    public List<string> VoiceLines = new List<string>();
}
