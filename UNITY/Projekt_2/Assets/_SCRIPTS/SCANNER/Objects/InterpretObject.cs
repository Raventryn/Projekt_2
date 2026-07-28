using UnityEngine;

public class InterpretObject : MonoBehaviour
{
    public ScannableObjectType ObjectType;
    public ScanObject[] InterpretationOptions = new ScanObject[3];

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onReplaceInterpretableObjects += ReplaceGameObject;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onReplaceInterpretableObjects -= ReplaceGameObject;
    }

    public void ShowButtonCanvas(bool toggle)
    {
        GameEventsManager.instance.inputEvents.ReleaseInteract();
        GameEventsManager.instance.questEvents.ShowButtonCanvas(toggle, ObjectType, InterpretationOptions);
    }

    void ReplaceGameObject(ScannableObjectType type, GameObject gameObject)
    {
        if(type != ObjectType) return;

        GameObject replacerObject = Instantiate(gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.transform.parent);
        replacerObject.transform.localScale = this.gameObject.transform.localScale;

        ScanObject newScanObject = replacerObject.GetComponent<ScanObject>();

        if(!ScannerManager.instance.ScannedObjects.ContainsKey(newScanObject.ObjectType))
            ScannerManager.instance.ScannedObjects.Add(newScanObject.ObjectType, true);

        else if(ScannerManager.instance.ScannedObjects.ContainsKey(newScanObject.ObjectType))
            ScannerManager.instance.ScannedObjects[newScanObject.ObjectType] = true;

        GameEventsManager.instance.interactionEvents.UpdateObjectScannedState(newScanObject.ObjectType);
        GameEventsManager.instance.questEvents.HideScanGlitch(newScanObject.ObjectType);

        Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }

    /*public void SendReplaceEvent(string objectName)
    {
        GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(ObjectType, ScannerManager.instance.ScannableObjects[objectName]);
        GameEventsManager.instance.soundEvents.TriggerSound(SoundType.GLITCH, false);
    }*/

}
