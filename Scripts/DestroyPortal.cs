using UnityEngine;
using System.Collections;

public class DestroyPortal : MonoBehaviour
{
    [Header("2: Red, 3: Green, 4: Blue")]
    public int DamageType = 1;

    private Collider trigger;
    private bool canDestroy = false;
    private bool isDestroyed = false;

    void Start()
    {
        trigger = GetComponent<Collider>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (!isDestroyed) //if has not yet been destroyed
        {
            if (coll.CompareTag("Player"))
            {
                canDestroy = true;
            }

            if (canDestroy)
            {
                if (coll.CompareTag("Attack"))
                {
                    if (coll.GetComponent<AoeAttack>() != null)
                    {
                        if(coll.GetComponent<AbstractAttack>().DamageType == DamageType)
                            ClosePortal();
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (!isDestroyed) //if has not yet been destroyed
        {
            if (coll.CompareTag("Player"))
            {
                canDestroy = false;
            }
        }
    }

    void ClosePortal()
    {
        isDestroyed = true;

        ParticleSystem[] particleSysArray;
        particleSysArray = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem p in particleSysArray)
        {
            p.playbackSpeed = 10;
            p.Stop();
        }

        GetComponent<AudioSource>().Stop();
        GetComponent<Spawner>().enabled = false;
        trigger.enabled = false;

        if(QuestManager.Current.QuestList[0].GetProgressOnTask(1) == 2) //if player has closed 2 portal, update with a new quest before completing task
        {
            GameManager.Current.ActivateStageOneVillage();
            QuestManager.Current.QuestList[0].AddTask(2, "Return to the village", 0);
        }

        QuestManager.Current.QuestList[0].AddProgressToTask(1);
    }
}
