//external functions
EXTERNAL StartQuest(Id)
EXTERNAL AdvanceQuest(Id)
EXTERNAL FinishQuest(Id)

//quest id (quest id + ID for variable name)
VAR DummyQuestId = "DummyQuest"

//quest states (quest Id + state for variable name)
VAR DummyQuestState = "REQUIREMENTS_NOT_MET"

INCLUDE DummyQuestTest.ink
INCLUDE TestStory.ink