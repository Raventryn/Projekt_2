using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FurnitureType
{
    DRAWER,
    DOOR
}

public enum OpenDirection
{
    RIGHT,
    LEFT
}

public class OpenFurniture : MonoBehaviour
{
    [SerializeField] List<GameObject> m_Drawers;
    [SerializeField] List<FurnitureType> m_Type;
    [SerializeField] List<OpenDirection> m_OpenDirection;

    Dictionary<GameObject, MoveOrRotateObject> drawerComponents = new Dictionary<GameObject, MoveOrRotateObject>();

    void OnEnable()
    {
        GameEventsManager.instance.interactionEvents.onOpenFurniture += OnOpenFurniture;
    }
    void OnDisable()
    {
        GameEventsManager.instance.interactionEvents.onOpenFurniture -= OnOpenFurniture;
    }

    void Start()
    {
        AddObjectComponents();
    }

    void AddObjectComponents()
    {
        for(int i = 0; i < m_Drawers.Count; i++)
        {
            drawerComponents.Add(m_Drawers[i], m_Drawers[i].AddComponent<MoveOrRotateObject>());
            drawerComponents[m_Drawers[i]].type = m_Type[i];
            drawerComponents[m_Drawers[i]].openDirection = m_OpenDirection[i];
        }
    }

    void OnOpenFurniture(GameObject gameObject, bool toggle)
    {

        if(gameObject != this.gameObject) return;
        
        for(int i = 0; i < m_Drawers.Count; i++)
        {
            switch (toggle)
            {
                case true:
                    switch (drawerComponents[m_Drawers[i]].type)
                    {
                        case FurnitureType.DRAWER:
                            drawerComponents[m_Drawers[i]].SetObjectTarget(OpenDrawerPosition(m_Drawers[i]));
                            break;
                        case FurnitureType.DOOR:
                            drawerComponents[m_Drawers[i]].SetObjectTarget(OpenDoorRotation(m_Drawers[i], i));
                            break;
                    }
                    break;
                case false:
                    switch (drawerComponents[m_Drawers[i]].type)
                        {
                            case FurnitureType.DRAWER:
                                drawerComponents[m_Drawers[i]].SetObjectTarget(drawerComponents[m_Drawers[i]].closedPosition);
                                break;
                            case FurnitureType.DOOR:
                                drawerComponents[m_Drawers[i]].SetObjectTarget(drawerComponents[m_Drawers[i]].closedRotation);
                                break;
                        }
                        break;
            }
            
        }
    }

    Vector3 OpenDrawerPosition(GameObject drawer)
    {
        BoxCollider collider = drawer.GetComponent<BoxCollider>();

        Vector3 openPosition = drawer.transform.localPosition + new Vector3(0f, 0f, collider.size .z * 0.75f);

        return openPosition;
    }

    Vector3 OpenDoorRotation(GameObject door, int index)
    {
        switch (drawerComponents[m_Drawers[index]].openDirection)
        {
            case OpenDirection.RIGHT:
                Vector3 openRotationR = door.transform.localEulerAngles - new Vector3(0, 100f, 0);
                return openRotationR;
    
            case OpenDirection.LEFT:
                Vector3 openRotationL = door.transform.localEulerAngles + new Vector3(0, 100f, 0);
                return openRotationL;
                
        }
         
         return Vector3.zero;
    }
}
