using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> onChangeSphereColour;

    public void ChangeSphereColour(string colour)
    {
        onChangeSphereColour?.Invoke(colour);
    }

    public event Action<bool, ScannableObjectType> onShowButtonCanvas;

    public void ShowButtonCanvas(bool toggle, ScannableObjectType type)
    {
        onShowButtonCanvas?.Invoke(toggle, type);
    }

    public event Action<ScannableObjectType, GameObject> onReplaceInterpretableObjects;

    public void ReplaceInterpretableObjects(ScannableObjectType type, GameObject replacerObject)
    {
        onReplaceInterpretableObjects?.Invoke(type, replacerObject);
    }
}
