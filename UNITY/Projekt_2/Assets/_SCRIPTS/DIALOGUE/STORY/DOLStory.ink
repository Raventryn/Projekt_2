===DOLStory===
{ RoachQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
    <wave>Hello darkness, my old friend</wave>
        ~AdvanceDialogueCamera("1")
    <wave>I've come to talk with you again</wave>
        ~AdvanceDialogueCamera("2")
    <wave>Because a vision softly creeping</wave>
        ~AdvanceDialogueCamera("3")
    <wave>Left its seeds while I was sleeping</wave>
        ~AdvanceQuest(RoachQuestId)
    -> END
= canStart
    Bla Bla pls kill 10 roaches thenks!
    Use your Scanner to <shake>grill</shake> them!
    ~StartQuest(RoachQuestId)
    ->END
= inProgress
    <shake> KILL THE ROACHES! </shake>
    ->END
= canFinish
    Did you kill all the roaches?
    <br>
    *[Yes!]
        <wave> THANK YOU!</wave>
        Here is the fish.
        Bring him back soon please.
        ~AddItem("INVENTORY:GUPPY")
        ~FinishQuest(RoachQuestId)
        
        -> END
    *[Nopers!]
        Oh...
        Ok then.
        ->END
=finished
    Thank you again for frying those roaches.
    -> END