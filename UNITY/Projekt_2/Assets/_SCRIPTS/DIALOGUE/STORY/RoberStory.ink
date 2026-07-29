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
    Hello.
    I am Rober.
    Bla Bla Janitor Bla Bla.
    Before I let you go we need to calibrate your extremities.
    Follow the movements of the projection with your hand.
    ~StartQuest(GameStartQuestId)
->END

= inProgressGameStart
    Well done.
    Your databanks are heavily corrupted.
    I have some datasets for you to restore some of the lost data.
    You will have to match the frequency of the incoming data.
    Adjust the wavelength and amplitude to the incoming signal.
    ~AdvanceQuest(GameStartQuestId)
->END

=canFinishGameStart
    Good job.
    Now your data should mostly be up to date.
    I have an old scanner here which you can use to complete your databanks.
    Now that you're back in working order I need you to do me a favor.
    The residents around here use fish to generate electricity.
    These fish need to be maintained and require frequent checkups.
    Could you remind everyone to bring their fish to me, so I can make sure everything is in order?
    //~AdvanceQuest(GameStartQuestId)
    ~FinishQuest(GameStartQuestId)
    ~SitPlayerUp()
    ~StartQuest(LonelinessFishQuestId)
    //Start Main Quest
->END

=finishedGameStart
    { LonelinessFishQuestState :
    - "IN_PROGRESS": -> waitingForFish
    - "CAN_FINISH": -> deliveringFish
    - "FINISHED": -> defaultDialogue
    - else: -> END
}
->END

=waitingForFish
    I still need that fish.
-> END

=deliveringFish
    Ah you have the fish.
    Poor little guy, I wish I had more of his kind.
    They get so lonely...
    Thank you for your help.
    ~FinishQuest(LonelinessFishQuestId)
    This is the end of the demo but feel free to keep exploring :)
    Here is the key to the other flats.
    ~AddItem("INVENTORY:MASTER_KEY")
->END

=defaultDialogue
    The developers didn't bother to give me any more voice lines...
   
->END
