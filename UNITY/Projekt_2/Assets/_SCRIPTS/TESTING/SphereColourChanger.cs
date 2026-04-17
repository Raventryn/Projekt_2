using System.Collections;
using Ink.Runtime;
using UnityEngine;

public class SphereColourChanger : MonoBehaviour
{
    [SerializeField] Renderer _renderer;

    void OnEnable()
    {
        GameEventsManager.instance.questEvents.onChangeSphereColour += ChangeColour;
    }

    void OnDisable()
    {
        GameEventsManager.instance.questEvents.onChangeSphereColour -= ChangeColour;
    }

    void Start()
    {
        ChangeColour("grey");
    }

    void ChangeColour(string colour)
    {
        switch (colour)
        {
            case "Grey":
                _renderer.material.color = Color.gray;
                GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable("Sphere_Colour", new StringValue("Grey"));
                break;
            case "Blue":
                _renderer.material.color = Color.blue;
                GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable("Sphere_Colour", new StringValue("Blue"));
                break;
            case "Green":
                _renderer.material.color = Color.green;
                GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable("Sphere_Colour", new StringValue("Green"));
                break;
        }

              
    }
}
