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
        
    <wave>I've come to talk with you again</wave>
        ~AdvanceDialogueCamera("2")
    <wave>Because a vision softly creeping</wave>
        ~AdvanceDialogueCamera("3")
    <wave>Left its seeds while I was sleeping</wave>
        ~AdvanceQuest(RoachQuestId)
    -> END
= canStart
    Oh another DOL-Unit.
    Where did he find you?
    <br>
        *[I don't remember.]
        Good for you.
        -> continueStart
    
=continueStart
    ~AdvanceDialogueCamera("2")
    I take it, you're moving into the apartment across?
    Rober lives next door, so prepare to get <wave>way too many</wave> unannounced visits.
    Otherwise it's quite peacful around here. 
    The others keep to themselves I think, but so do I.
    But .. 
    ~AdvanceDialogueCamera("1")
    But those <shake a*2>damned</shake> tiny animals don't seem to get that.

    <br>
    *[What are they?]
         Privacy indvading, bothersome little shits. I've told them countless times that I don't want to be their friend.
         ~AdvanceDialogueCamera("0")
         <wave a * 0.5f>I don't make friends.</wave>
         -> finishStart
        
    
= finishStart
    <br>
    *[Should I try to uninvite them?]
        Suit yourself.
        ~StartQuest(RoachQuestId)
        ->END
    
= inProgress
    I don't really .. talk a lot.
    Can you get rid of them already?
    ->END
= canFinish
    Did you get rid of them yet?
    <br>
    *[They're gone!]
        Oh...
        Well, um .. thank you then.
        You're good at convincing people, it seems. 
        ...
        Please don't do that with me. 
        ->continueFinish
        
= continueFinish
    *[Actually Rober asked me to bring him your fish.]
        Oh.
        Time flies really fast lately.
        I'm not sure, well ..
        You can take him... but bring him back as soon as you can.
        He doesn't like being around other fish for too long.

        ~AddItem("INVENTORY:GUPPY")
        ~RemoveFishFromTank("GUPPY")
        ~FinishQuest(RoachQuestId)
        
        -> END

=finished
    Do you think he enjoys his time away from me?
    I get the feeling he doesn't want to be around other fish.
    ~AdvanceDialogueCamera("2")
    Why else would Rober have given him to me?
    And ..
    ~AdvanceDialogueCamera("0")
    His colors change so much when he's gone. 
    Can't be good. 
    -> END