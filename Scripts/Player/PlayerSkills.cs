using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerSkills : MonoBehaviour
{
    public static PlayerSkills Current;

    public Slider AoeCooldownSlider;
    public Slider BeamCooldownSlider;

    public int AoeCooldownSeconds;
    public int BeamCooldownSeconds;

    public bool IsAoeReady = true;
    public bool IsBeamReady = true;

    private bool hasFlashedAoe = true;
    private bool hasFlashedBeam = true;

    public PlayerSkills()
    {
        Current = this;
    }

    void Start()
    {
        AoeCooldownSlider.maxValue = AoeCooldownSeconds;
        BeamCooldownSlider.maxValue = BeamCooldownSeconds;

        AoeCooldownSlider.value = 0;
        BeamCooldownSlider.value = 0;
    }

    void Update()
    {
        UpdateCooldown(AoeCooldownSlider);
        UpdateCooldown(BeamCooldownSlider);
    }

    public void ActivateAoe()
    {
        IsAoeReady = false;
        AoeCooldownSlider.value = AoeCooldownSlider.maxValue;
        AoeCooldownSlider.GetComponentInChildren<Image>().color = Color.white;
    }

    public void ActivateBeam()
    {
        IsBeamReady = false;
        BeamCooldownSlider.value = BeamCooldownSlider.maxValue;
        BeamCooldownSlider.GetComponentInChildren<Image>().color = Color.white;
    }

    void FlashSkill(Slider slider)
    {
        slider.GetComponentInChildren<Image>().color = Color.black;
    }

    void UpdateCooldown(Slider slider)
    {
        if (slider.value > slider.minValue) //on cooldown
        {
            slider.value -= Time.deltaTime;

            if (slider == AoeCooldownSlider)
            {
                hasFlashedAoe = false;

            }
            else if (slider == BeamCooldownSlider)
            {
                hasFlashedBeam = false;
            } 
        }

        else 
        {
            if (slider == AoeCooldownSlider && !hasFlashedAoe)
            {
                FlashSkill(slider);
                hasFlashedAoe = true;
                IsAoeReady = true;
            }
            else if (slider == BeamCooldownSlider && !hasFlashedBeam)
            {
                FlashSkill(slider);
                hasFlashedBeam = true;
                IsBeamReady = true;
            }
            else
            {
                slider.GetComponentInChildren<Image>().color = Color.Lerp(slider.GetComponentInChildren<Image>().color, Color.white, 5 * Time.deltaTime);
            }
        }
    }
}
