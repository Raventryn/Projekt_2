using UnityEngine;

public class TriggerMinigame : MonoBehaviour
{
    public void OpenMinigame(bool toggle, ScannableObjectType type)
    {
        GameEventsManager.instance.questEvents.StartScanMinigame(toggle, type);
    }
}
