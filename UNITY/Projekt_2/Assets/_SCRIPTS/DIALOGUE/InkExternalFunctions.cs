using Ink.Runtime;
using UnityEngine;

public class InkExternalFunctions
{
    public void Bind(Story story)
    {
        story.BindExternalFunction("ChangeSphereColour", (string colour) => ChangeSphereColour(colour));
        story.BindExternalFunction("StartQuest", (string Id) => StartQuest(Id));
        story.BindExternalFunction("AdvanceQuest", (string Id) => AdvanceQuest(Id));
        story.BindExternalFunction("FinishQuest", (string Id) => FinishQuest(Id));
        //story.BindExternalFunction("SendSpitEvent", () => SendSpitEvent());
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("ChangeSphereColour");
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
        //story.UnbindExternalFunction("SendSpitEvent");
    }

    private void ChangeSphereColour(string colour)
    {
        GameEventsManager.instance.questEvents.ChangeSphereColour(colour);
    }

    private void StartQuest(string id)
    {
        GameEventsManager.instance.questEvents.StartQuest(id);
    }

    private void AdvanceQuest(string id)
    {
        GameEventsManager.instance.questEvents.AdvanceQuest(id);
    }

    private void FinishQuest(string id)
    {
        GameEventsManager.instance.questEvents.FinishQuest(id);
    }

}
