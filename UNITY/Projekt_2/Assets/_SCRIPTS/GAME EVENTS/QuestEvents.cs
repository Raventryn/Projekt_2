using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> onChangeSphereColour;

    public void ChangeSphereColour(string colour)
    {
        onChangeSphereColour?.Invoke(colour);
    }
}
