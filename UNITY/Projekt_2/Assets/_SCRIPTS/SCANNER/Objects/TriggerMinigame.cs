using UnityEngine;

public class TriggerMinigame : MonoBehaviour
{
    public void OpenMinigame(bool toggle)
    {
        GameEventsManager.instance.questEvents.StartScanMinigame(toggle);
    }
}
