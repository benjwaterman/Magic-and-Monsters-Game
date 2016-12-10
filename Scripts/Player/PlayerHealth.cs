using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;                            // The amount of health the player starts the game with.
    public int CurrentHealth;                                   // The current health the player has.
    public Slider HealthSlider;                                 // Reference to the UI's health bar.
    public Image DamageImage;                                   // Reference to an image to flash on the screen on being hurt.
    public AudioClip DeathClip;                                 // The audio clip to play when the player dies.
    public float FlashSpeed = 5f;                               // The speed the damageImage will fade at.
    public Color FlashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.
    public Text DeathText;


    Animator animator;                                              // Reference to the Animator component.
    AudioSource playerAudio;                                    // Reference to the AudioSource component.
    //PlayerMovement playerMovement;                              // Reference to the player's movement.
    //PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
    [HideInInspector]public static bool isDead;                                                // Whether the player is dead.
    bool damaged;                                               // True when the player gets damaged.


    void Awake()
    {
        // Setting up the references.
        animator = GetComponent<Animator>();
        //playerAudio = GetComponent<AudioSource>();
        //playerMovement = GetComponent<PlayerMovement>();
        //playerShooting = GetComponentInChildren<PlayerShooting>();

        // Set the initial health of the player.
        CurrentHealth = MaxHealth;
        HealthSlider.value = CurrentHealth;
        DeathText.GetComponent<CanvasGroup>().alpha = 0;
    }


    void Update()
    {
        // If the player has just been damaged...
        if (damaged)
        {
            // ... set the colour of the damageImage to the flash colour.
            
            DamageImage.color = FlashColour;
        }
        // Otherwise...
        else 
        {
            animator.SetInteger("Damage", 0);

            if (!isDead)
                DamageImage.color = Color.Lerp(DamageImage.color, Color.clear, FlashSpeed * Time.deltaTime);
        }

        if (isDead)
        {
            DamageImage.color = Color.Lerp(DamageImage.color, Color.black, Time.deltaTime);
            DeathText.GetComponent<CanvasGroup>().alpha += 0.5f * Time.deltaTime;
        }

        // Reset the damaged flag.
        damaged = false;
    }


    public void TakeDamage(int amount)
    {
        if (amount >= 25)
        {
            animator.SetInteger("Damage", 2); //large hit
            PlayerAttack.Current.TriggerInterrupt();
        }
        else
            animator.SetInteger("Damage", 1); //small hit

        // Set the damaged flag so the screen will flash.
        damaged = true;

        // Reduce the current health by the damage amount.
        CurrentHealth -= amount;

        // Set the health bar's value to the current health.
        HealthSlider.value = CurrentHealth;

        // Play the hurt sound effect.
        //playerAudio.Play();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if (CurrentHealth <= 0 && !isDead)
        {
            // ... it should die.
            Death();
        }           
    }

    public void GainHealth(int amount)
    {
        if ((CurrentHealth += amount) > MaxHealth)
            CurrentHealth = MaxHealth;
        HealthSlider.value = CurrentHealth;
    }


    void Death()
    {
        isDead = true;

        animator.SetTrigger("Dead");
        MagicBallOrbit.Current.DisableAllOrbs();
        PlayerController.Current.DisableCameraMovement();
        GameManager.Current.PlayFailureSound();

        DamageImage.transform.SetParent(GameManager.Current.OverlayCanvas.transform); //so it doesnt become invisible too
        PlayerController.Current.HudCanvas.GetComponent<CanvasGroup>().alpha = 0;
        DeathText.transform.SetAsLastSibling();

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        //playerAudio.clip = DeathClip;
        //playerAudio.Play();
    }
}