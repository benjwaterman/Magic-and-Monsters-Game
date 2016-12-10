using UnityEngine;
using System.Collections;
using System;

public class DoorInteractionTrigger : InteractionTrigger
{
    private TalkInteraction conversation;
    private bool hasTalked = false;

    void Start()
    {
        conversation = GetComponent<TalkInteraction>();
    }

    void Update()
    {

    }

    public override void Play(int index)
    {
        ConversationIndex = conversation.ConversationIndex; //which conversation are we in
        switch (ConversationIndex)
        {
            case 0:
                switch (index)
                {
                    case 0:
                        if (!hasTalked)
                        {
                            QuestManager.Current.QuestList[1].AddProgressToTask(0);
                            hasTalked = true;
                        }
                        break;
                }
                break;
        }
    }
}
