//external functions
EXTERNAL StartQuest(Id)
EXTERNAL AdvanceQuest(Id)
EXTERNAL FinishQuest(Id)
EXTERNAL AdvanceDialogueCamera(cameraStage)
EXTERNAL SitPlayerUp()
EXTERNAL AddItem(Id)
EXTERNAL RemoveItem(Id)

//quest id (quest id + ID for variable name)
VAR DummyQuestId = "DummyQuest"
VAR RoachQuestId = "RoachQuest"
VAR GameStartQuestId = "GameStartQuest"
VAR LonelinessFishQuestId = "LonelinessFishQuest" 

//quest states (quest Id + state for variable name)
VAR DummyQuestState = "REQUIREMENTS_NOT_MET"
VAR RoachQuestState = "REQUIREMENTS_NOT_MET"
VAR GameStartQuestState = "REQUIREMENTS_NOT_MET"
VAR LonelinessFishQuestState = "CAN_START"

INCLUDE DummyQuestTest.ink
INCLUDE TestStory.ink
INCLUDE RoberStory.ink
INCLUDE DOLStory.ink