using Ink.Runtime;
using UnityEngine;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("ChangeSphereColour", (string colour) => ChangeSphereColour(colour));
        //story.BindExternalFunction("SendSpitEvent", () => SendSpitEvent());
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("ChangeSphereColour");
        //story.UnbindExternalFunction("SendSpitEvent");
    }

    private void ChangeSphereColour(string colour)
    {
        GameEventsManager.instance.questEvents.ChangeSphereColour(colour);
    }

}
