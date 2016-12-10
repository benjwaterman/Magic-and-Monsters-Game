using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Current;

    public GameManager()
    {
        Current = this;
    }

    [Header("Conversation Related Props")]
    public Text NPCText;
    public Text NPCName;
    public Text PlayerText;
    public GameObject GO_CinematicCamera;

    [Header("Sound")]
    public AudioSource AmbientSource;
    public AudioSource EffectSource;
    public AudioClip AchievementSound;
    public AudioClip CompletionSound;
    public AudioClip FailureSound;

    [Header("UI")]
    public Canvas OverlayCanvas;

    [Header("Village Props")]
    public GameObject[] VillageHouseArray;
    public GameObject VillageNPCContainer;
    public GameObject VillageSkyPortalContainer;
    public Light VillageTreeLight1;
    public Light VillageTreeLight2;
    public GameObject Gandol;

    private bool reduceVolume = false;
    
    void Awake()
    {
        VillageSkyPortalContainer.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown("j"))
        {
            ActivateStageOneVillage();
        }

        if(reduceVolume)
        {
            if (AudioListener.volume <= 0.15f)
            {
                reduceVolume = false;
            }

            AudioListener.volume -= 0.5f * Time.deltaTime;
            Camera.main.GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration>().intensity += 0.2f * Time.deltaTime;
        }
    }

    public void PlayAchievementSound()
    {
        if (!EffectSource.isPlaying)
        {
            EffectSource.clip = AchievementSound;
            EffectSource.Play();
        }
    }

    public void PlayCompletionSound()
    {
        if (!EffectSource.isPlaying)
        {
            EffectSource.clip = CompletionSound;
            EffectSource.Play();
        }
    }

    public void PlayFailureSound()
    {
        if (!EffectSource.isPlaying)
        {
            EffectSource.volume = 1;
            EffectSource.clip = FailureSound;
            EffectSource.Play();
        }
    }

    public void ActivateStageOneVillage()
    {
        //foreach (GameObject house in VillageHouseArray)
        //{
        //    HouseLightController lighController = house.GetComponent<HouseLightController>();
        //    lighController.LightColor = Color.red;
        //    lighController.BaseIntensity = 5;
        //    lighController.FlickerAmount = 10;
        //    lighController.FlickerFrequency = 4;
        //}

        VillageTreeLight1.color = Color.red;
        VillageTreeLight2.color = Color.red;
        VillageSkyPortalContainer.SetActive(true);
        VillageNPCContainer.SetActive(false);
        Gandol.GetComponent<Animator>().SetTrigger("Dead");
        Gandol.GetComponent<Collider>().enabled = false;
        TriggerConfusion();
    }

    public void TriggerConfusion()
    {
        reduceVolume = true;
    }
}


