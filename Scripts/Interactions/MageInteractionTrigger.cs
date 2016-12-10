using UnityEngine;
using System.Collections;
using System;

public class MageInteractionTrigger : InteractionTrigger
{
    private int currentTriggerLocation = 0;
    private Animator animator;
    private GameObject GO_CinematicCamera;
    private bool rotateToPortal = false;
    private TalkInteraction conversation;

    void Start()
    {
        animator = GetComponent<Animator>();
        GO_CinematicCamera = GameManager.Current.GO_CinematicCamera; //get cinetmatic camera from other script
        conversation = GetComponent<TalkInteraction>();
    }

    void Update()
    {
        if (rotateToPortal)
        {
            Vector3 targetDir = SkyPortalController.Current.GreenPortal.transform.position - GO_CinematicCamera.transform.position;
            var rotation = Quaternion.LookRotation(targetDir);
            GO_CinematicCamera.transform.rotation = Quaternion.Slerp(GO_CinematicCamera.transform.rotation, rotation, Time.deltaTime * 1);
            GO_CinematicCamera.transform.eulerAngles = new Vector3(GO_CinematicCamera.transform.eulerAngles.x, GO_CinematicCamera.transform.eulerAngles.y, 0); //stop camera rotating sideways
        }
    }

    public override void Play(int index)
    {
        ConversationIndex = conversation.ConversationIndex; //which conversation are we in
        switch (ConversationIndex)
        {
            case 0:
                switch (index)
                {
                    case 0: //look at portal
                        animator.SetTrigger("Point");
                        SkyPortalController.Current.EnableAllPortals();
                        //GameManager.Current.ActivateStageOneVillage();
                        rotateToPortal = true;
                        break;

                    case 1: //look back at npc
                        rotateToPortal = false;
                        conversation.PositionCamera();
                        break;

                    case 2:
                        QuestManager.Current.QuestList[0].AddTask(1, "Close the portals", 3);
                        QuestManager.Current.QuestList[0].AddProgressToTask(0);                      
                        break;
                }
                break;
        }

    }
}
