using UnityEngine;

public class InterpretObject : MonoBehaviour
{
    public ScannableObjectType ObjectType;

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
        GameEventsManager.instance.questEvents.ShowButtonCanvas(toggle, ObjectType);
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

        Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }

    public void SendReplaceEvent(string objectName)
    {
        GameEventsManager.instance.questEvents.ReplaceInterpretableObjects(ObjectType, ScannerManager.instance.ScannableObjects[objectName]);
    }

}
