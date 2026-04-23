using System.Collections.Generic;
using UnityEngine;

public class ScannerManager : MonoBehaviour
{
    public static ScannerManager instance;

    public Dictionary<ScannableObjectType,bool> ScannedObjects;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Multiple Scanner Managers loaded");
            return;
        }
        else
        {
            instance = this;
            ScannedObjects = new Dictionary<ScannableObjectType, bool>();
        }
    }
}
