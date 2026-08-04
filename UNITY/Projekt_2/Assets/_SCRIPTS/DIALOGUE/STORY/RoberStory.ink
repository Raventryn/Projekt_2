===RoberStory===

{ GameStartQuestState :
    - "CAN_START": -> canStartGameStart
    - "IN_PROGRESS": -> inProgressGameStart
    - "CAN_FINISH": -> canFinishGameStart
    - "FINISHED": -> finishedGameStart
    - else: -> finishedGameStart
}
//~StartQuest(LonelinessFishQuestId)
//->finishedGameStart


= canStartGameStart
    <wiggle>Hello?</wiggle>
    I did not expect you to actually boot.
    That drive I installed was almost as corrupted as the one you had before.
    ~AdvanceDialogueCamera("1")
    Can you talk?
    <br>
    *[Hello World!]
        ->continueGameStart
    *[Quack!]
        ->continueGameStart
    *[Yes.]
        ->continueGameStart
->END

=continueGameStart
    ~AdvanceDialogueCamera("0")
    <bounce s*2>Eureka!</bounce>
    Alright, so before I overwhelm you with too much information. Let's complete your setup first!
    Can you move your arms?
    ~StartQuest(GameStartQuestId)

-> END

= inProgressGameStart
    <slideh>Well done.</slideh>
    ~AdvanceDialogueCamera("2")
    Your main data storage was heavily damaged, because you were exposed to the cold for far too long.
    I installed a new one, but its also somewhat broken and incomplete. 
    I will load up some additional data packages, let's see if that fixes the problem?
    
    ~AdvanceQuest(GameStartQuestId)
->END

=canFinishGameStart
    <wave>Hmmm...</wave>
    ~AdvanceDialogueCamera("1")
    Seems like it worked, but you're still missing some data.
    You might come across some visual glitches.
    But I think for now you should be well enough to be let loose and experience the world.
    ~AdvanceDialogueCamera("0")
    I am <wave>Rober</wave>, the caretaker of this community.
    Your serial number is <wave>DOL-179</wave>, looks like you were a companion robot, just like our <wave>Claire</wave>...  
    Maybe you will help her get out of her shell.
    Since the other humans went away, I've been repairing robots and providing them with a place to live.
    I'm sure you will fit in <wave>nicely.</wave>
   ~AdvanceDialogueCamera("2") 
    There's still some preparations to make for your new apartment and your welcome gift isn't even ready yet.
    Will you do something for me in the meantime?
    
    <br>
    
    *[Already?!]
        <shake>Hah very funny!</shake> I always liked the humour of you companion robots!
        -> finishGameStart
    *[Of course!]
        Thank you! I always found you companion robots' willingness to help very human like.
        -> finishGameStart
    
=finishGameStart
    ~AdvanceDialogueCamera("0")
    You may still encounter visual glitches, as we couldn't restore your data fully.
    To change that, you can have my old Scanner to help with processing what you can't identify.
    Helped me many times before, <wave>these circuts don't get any younger, haha.</wave>
    Why don't you check out your new home and introduce yourself to everyone while repairing your data?
    ...
    ~AdvanceDialogueCamera("1")
    Oh and also, can your remind them to bring their fish to me, for another checkup?
    <br>
    *[Fish?]
        We use water movement from fishes swimming around their tanks, to keep the lights on around here.
        Everyone gets their own personal one.
       <wave>But don't you worry about that yet!</wave>
       ~AdvanceDialogueCamera("0")
    Off, off you go now! <swing>Have fun!</swing>
    (The devs only finished one character and quest for this demo, meet her in the apartment on the other end of the hallway lol)
    
    //~AdvanceQuest(GameStartQuestId)
    ~FinishQuest(GameStartQuestId)
    ~SitPlayerUp()
    ~StartQuest(LonelinessFishQuestId)
    //Start Main Quest
-> END
    
=finishedGameStart
    { LonelinessFishQuestState :
    - "IN_PROGRESS": -> waitingForFish
    - "CAN_FINISH": -> deliveringFish
    - "FINISHED": -> defaultDialogue
    - else: -> END
}
->END

=waitingForFish
    I'm a little busy right now, finishing up your stuff. 
    Do you have the fishes yet?
    *[Not yet]
        <incr>Seems like you're just as busy then!</incr>
-> END

=deliveringFish
    I'm a little busy right now, finishing up your stuff. 
    Do you have the fishes yet?
    <br>
    *[Not yet]
        <incr>Seems like you're just as busy then!</incr>
        ->END
    *[I've got them!]
        <wave>Thank you very much!</wave> Time to check these fellas out!
    ~FinishQuest(LonelinessFishQuestId)
    ~AdvanceDialogueCamera("1")
    This is the end of the demo but feel free to keep exploring :)
    Here is the key to the other apartments, if you want to check them out.
    <bounce>Thank you for playing!</bounce>
    (you can close the game when you're done)
    ~AddItem("INVENTORY:MASTER_KEY")
->END

=defaultDialogue
    The developers didn't bother to give me any more voice lines.
   
->END
