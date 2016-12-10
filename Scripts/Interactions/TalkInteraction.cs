using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TalkInteraction : Interactive
{
    public List<ConversationWrapper> ConversationContainer = new List<ConversationWrapper>();
    public InteractionTrigger TriggerScript;
    public float CameraDistanceBack = 1;
    public float CameraDistanceUp = 1.8f;

    private Text NPCText;
    private Text NPCName;
    private Text PlayerText;
    private GameObject GO_CinematicCamera;

    [NonSerialized]
    public int ConversationIndex = 0; //current conversation index
    [NonSerialized]
    public int SubConversationIndex = 0; //index within current conversation
    [NonSerialized]
    public int TriggerIndex = 0; //which trigger we are currently on

    private Animator animator;
    private Camera mainCamera;
    private Camera cinematicCamera;

    void Awake()
    {
        NPCText = GameManager.Current.NPCText;
        NPCName = GameManager.Current.NPCName;
        PlayerText = GameManager.Current.PlayerText;
        GO_CinematicCamera = GameManager.Current.GO_CinematicCamera;

        Interaction = InteractionType.Talk; //make sure interaction type is set to talk
        animator = GetComponent<Animator>();
        cinematicCamera = GO_CinematicCamera.GetComponent<Camera>();
        mainCamera = Camera.main;

        cinematicCamera.enabled = false;
    }

    public void InitialiseConversation() //set converstaion to first string in list
    {
        SubConversationIndex = 0; //reset where in conversation we are when we start talking 
        TriggerIndex = 0;

        NPCName.text = GetComponent<CharacterInfo>().Name;
        NPCText.text = ConversationContainer[ConversationIndex].Conversation[SubConversationIndex++];
        PlayerText.text = ConversationContainer[ConversationIndex].Conversation[SubConversationIndex];

        if(animator != null)
            animator.SetBool("Talking", true);

        cinematicCamera.enabled = true;
        mainCamera.enabled = false;
        PositionCamera();
    }

    public void Continue()
    {
        if (SubConversationIndex < ConversationContainer[ConversationIndex].Conversation.Count - 1)
        {
            SubConversationIndex++;

            if (Array.Exists(ConversationContainer[ConversationIndex].TriggerIndex, element => element.Equals(SubConversationIndex))) //check current list location to see if it is a trigger
            {
                TriggerScript.Play(TriggerIndex++);
            }

            NPCText.text = ConversationContainer[ConversationIndex].Conversation[SubConversationIndex++];

            if (Array.Exists(ConversationContainer[ConversationIndex].TriggerIndex, element => element.Equals(SubConversationIndex))) //check current list location to see if it is a trigger
            {
                TriggerScript.Play(TriggerIndex++);
            }

            PlayerText.text = ConversationContainer[ConversationIndex].Conversation[SubConversationIndex];
        }

        else //exit conversation
        {
            PlayerController.Current.StopTalking();
            if(animator != null)
                animator.SetBool("Talking", false);
            cinematicCamera.enabled = false;
            mainCamera.enabled = true;

            if (ConversationContainer[ConversationIndex].NextConversationOnEnd)
                TriggerNextConversation();
        }
    }

    public void PositionCamera()
    {
        GO_CinematicCamera.transform.position = this.transform.position + (this.transform.forward * CameraDistanceBack) + (this.transform.up * CameraDistanceUp); //position camera in front of character and up 
        Vector3 whereToLook = this.transform.position;
        whereToLook.y = GO_CinematicCamera.transform.position.y; //so camera looks straight ahead
        GO_CinematicCamera.transform.LookAt(whereToLook);
    }

    public void TriggerNextConversation()
    {
        if (ConversationContainer.Count > ConversationIndex + 1)
            ConversationIndex++;

        else
            Debug.Log("No more conversations");
    }
}

[System.Serializable]
public class ConversationWrapper
{
    public List<string> Conversation;
    public int[] TriggerIndex;
    public bool NextConversationOnEnd;
}
