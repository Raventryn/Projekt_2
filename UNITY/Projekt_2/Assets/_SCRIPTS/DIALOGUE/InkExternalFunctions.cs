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
        story.BindExternalFunction("AdvanceDialogueCamera", (string cameraStage) => AdvanceDialogueCamera(cameraStage));
        story.BindExternalFunction("SitPlayerUp", () => SitPlayerUp());
        story.BindExternalFunction("AddItem", (string Id) => AddItem(Id));
        story.BindExternalFunction("RemoveItem", (string Id) => RemoveItem(Id));
        //story.BindExternalFunction("SendSpitEvent", () => SendSpitEvent());
    }

    public void Unbind(Story story)
    {
        story.UnbindExternalFunction("ChangeSphereColour");
        story.UnbindExternalFunction("StartQuest");
        story.UnbindExternalFunction("AdvanceQuest");
        story.UnbindExternalFunction("FinishQuest");
        story.UnbindExternalFunction("AdvanceDialogueCamera");
        story.UnbindExternalFunction("SitPlayerUp");
        story.UnbindExternalFunction("AddItem");
        story.UnbindExternalFunction("RemoveItem");
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

    private void AdvanceDialogueCamera(string cameraStage)
    {
        GameEventsManager.instance.dialogueEvents.AdvanceDialogueCamera(cameraStage);
    }

    private void SitPlayerUp()
    {
        GameEventsManager.instance.questEvents.SitPlayerUp();
    }

    private void AddItem(string Id)
    {
        GameEventsManager.instance.inventoryEvents.AddItem(Id);
    }

    private void RemoveItem(string Id)
    {
        GameEventsManager.instance.inventoryEvents.RemoveItem(Id);
    }
}
