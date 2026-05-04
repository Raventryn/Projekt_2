
=== DummyQuestStart ===
{ DummyQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
    I don't have anything for you to do right now. Come back later.
    ~AdvanceQuest(DummyQuestId)
    -> END

= canStart
    You wanna test the quest?
    
    <br>
    
    *[Sure!]
        Good, then <incr>scan 2</incr> objects and afterwards <incr>interpret 1</incr>!
        ~StartQuest(DummyQuestId)
        -> END
    *[No thanks!]
        -> END
        
= inProgress
    Shouldn't you be doing your task?
    -> END
    
= canFinish
    Shouldn't you be doing your task?
    <br>
    *[I'm done!]
        Good job, very nice!
        ~FinishQuest(DummyQuestId)
        -> END

= finished
    I don't have any more work for you. <shake>Go away</shake>!
    -> END