using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Cameras;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;

    public GameObject HudCanvas;
    public GameObject ConversationCanvas;

    public GameObject CrossHair;
    public GameObject TalkIndicator;
    public GameObject TargetInfo;
    public Text TargetName;
    public Slider TargetHealthSlider;

    public Light PlayerLight;
    public ParticleSystem AirParticles;
    public GameObject GO_CameraRig;

    public float FootstepFrequency;
    private float timeSinceLastFootstep = 0;

    [Header("Sound")]
    public AudioSource FootstepAudio;
    public AudioSource AttackAudio;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public bool InCombat = false;
    [HideInInspector]
    public TalkInteraction TalkTarget;
    [HideInInspector]
    public bool IsAttacking = false;
    [HideInInspector]
    public bool IsNotBusy { get; private set; }

    private float vInput;
    private float hInput;
    private float prevMouseX;
    private float speedMultiplier;
    private float talkDistance = 1.5f;
    private float cameraTurnSpeed = 250;
    private bool isTalking = false;
    private bool fadeIn = false;
    private ProtectCameraFromWallClip wallClip;
    private FreeLookCam lookCam;
    private CanvasGroup targetInfoCanvas;
    private PlayerSkills playerSkills;
    private CharacterController playerCharController;


    public PlayerController()
    {
        Current = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        prevMouseX = Input.GetAxis("Mouse X");
        speedMultiplier = 1f;

        wallClip = Camera.main.transform.parent.parent.GetComponent<ProtectCameraFromWallClip>();
        lookCam = Camera.main.transform.parent.parent.GetComponent<FreeLookCam>();

        targetInfoCanvas = TargetInfo.GetComponent<CanvasGroup>();

        playerSkills = PlayerSkills.Current;

        DisableTargetInfo();

        TalkIndicator.SetActive(false);
        EnableHud();
        DisableConversationUI();

        IsNotBusy = true;

        playerCharController = GetComponent<CharacterController>();

        AttackAudio.volume = 0;
    }

    void Update()
    {
        if (PlayerHealth.isDead) //if dead, do nothing past this 
            return;

        if (isTalking) //if talking, do nothing past this
        {
            animator.SetFloat("VSpeed", 0);
            animator.SetFloat("HSpeed", 0);
            return;
        }

        //input
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        //press right click
        if (Input.GetMouseButtonDown(1))
        {
            InCombat = true;
            lookCam.m_MoveSpeed = 20;
            cameraTurnSpeed = 10000;
        }

        //holding right click
        else if (Input.GetMouseButton(1))
        {
            animator.SetLayerWeight(0, Mathf.Lerp(animator.GetLayerWeight(0), 0, 0.05f));
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, 0.05f));
            wallClip.m_OriginalDist = Mathf.Lerp(wallClip.m_OriginalDist, 1, 0.05f);
        }

        //releasing right click or not holding down right click
        else if (Input.GetMouseButtonUp(1) || !Input.GetMouseButton(1))
        {
            if (!IsAttacking)
            {
                InCombat = false;
                animator.SetLayerWeight(0, Mathf.Lerp(animator.GetLayerWeight(0), 1, 0.05f));
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, 0.05f));

                wallClip.m_OriginalDist = 2;
                lookCam.m_MoveSpeed = 10;

                cameraTurnSpeed = 175;
            }
        }

        IsNotBusy = !IsAttacking && !animator.GetCurrentAnimatorStateInfo(0).IsTag("NoAttack");

        //NOT IN COMBAT
        if (InCombat == false)
        {
            HandleOutOfCombat();
        }

        //IN COMBAT
        else
        {
            HandleInCombat();
        }

        //NON COMBAT EXCLUSIVE
        HandleOther();
        HandleSound();
    }

    void UpdateTargetInfo(string name, int health, int maxHealth)
    {
        targetInfoCanvas.alpha = 1;

        TargetName.text = name;
        TargetHealthSlider.maxValue = maxHealth;
        TargetHealthSlider.value = health;
    }

    void DisableTargetInfo()
    {
        targetInfoCanvas.alpha = 0;
    }

    void HandleInCombat() //during combat
    {
        CrossHair.SetActive(true);
        TalkIndicator.SetActive(false);

        animator.SetFloat("HSpeed", Mathf.Lerp(animator.GetFloat("HSpeed"), hInput * speedMultiplier, 0.1f));
        animator.SetFloat("VSpeed", Mathf.Lerp(animator.GetFloat("VSpeed"), vInput * speedMultiplier, 0.1f));

        transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0), Time.deltaTime * cameraTurnSpeed);

        //what player is targeting
        RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Friendly")) //cant shoot friendly character
            {
                CrossHair.GetComponent<Image>().color = Color.green;

                CharacterInfo info = hit.transform.GetComponent<CharacterInfo>();
                UpdateTargetInfo(info.Name, info.CurrentHealth, info.MaxHealth);

                return; // prevent shooting friendly characters
            }

            else if (hit.transform.CompareTag("Enemy"))
            {
                CrossHair.GetComponent<Image>().color = Color.red;

                CharacterInfo info = hit.transform.GetComponent<CharacterInfo>();
                UpdateTargetInfo(info.Name, info.CurrentHealth, info.MaxHealth);
            }

            else
            {
                CrossHair.GetComponent<Image>().color = Color.white;
                targetInfoCanvas.alpha = 0;
            }
        }
    }

    void HandleOutOfCombat() //not in combat
    {
        CrossHair.SetActive(false);
        DisableTargetInfo();

        Vector2 inputVector = new Vector2(vInput, hInput);
        inputVector = Vector2.ClampMagnitude(inputVector, 1);

        //moving forwards
        if (inputVector.x >= 0)
        {
            animator.SetFloat("VSpeed", Mathf.Lerp(animator.GetFloat("VSpeed"), inputVector.magnitude * speedMultiplier, 0.1f));

            if (inputVector.y > 0 || inputVector.y < 0)
                transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.Euler(0, Camera.main.transform.eulerAngles.y + 90 * inputVector.y, 0), Time.deltaTime * cameraTurnSpeed);
        }
        //moving backwards
        else if (inputVector.x < 0)
        {
            animator.SetFloat("VSpeed", Mathf.Lerp(animator.GetFloat("VSpeed"), -inputVector.magnitude * speedMultiplier, 0.1f));

            if (inputVector.y > 0 || inputVector.y < 0)
                transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.Euler(0, Camera.main.transform.eulerAngles.y - 90 * inputVector.y, 0), Time.deltaTime * cameraTurnSpeed);
        }

        //rotate player to face camera facing direction, when player is moving forward and not moving left or right
        if (vInput != 0 && hInput == 0)
            transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0, transform.eulerAngles.y, 0), Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0), Time.deltaTime * cameraTurnSpeed);

        //jumping
        if (Input.GetButtonDown("Jump") && IsNotBusy == true)
            animator.SetBool("Jumping", true);

        //talking
        Debug.DrawLine(transform.position + new Vector3(0, 1.5f, 0), transform.position + new Vector3(0, 1.5f, 0) + transform.forward * 1, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 1.5f, 0), transform.forward, out hit, talkDistance))
        {
            if (hit.transform.CompareTag("Friendly") && hit.transform.GetComponent<TalkInteraction>()) //if friendly and interactive
            {
                TalkIndicator.SetActive(true);

                if (Input.GetKeyDown("f") && IsNotBusy == true) //if not attacking and press f
                {
                    Talk(hit.transform.GetComponent<TalkInteraction>());
                }
            }

            else
                TalkIndicator.SetActive(false);
        }

        else
            TalkIndicator.SetActive(false);
    }

    void HandleOther() //non combat related
    {
        //red
        if (Input.GetKeyDown("1"))
        {
            PlayerAttack.Current.CurrentColor = PlayerAttack.Current.ColorRed;
            PlayerAttack.Current.ChangeAttackColor(PlayerAttack.Current.CurrentColor);
        }
        //green
        if (Input.GetKeyDown("2"))
        {
            PlayerAttack.Current.CurrentColor = PlayerAttack.Current.ColorGreen;
            PlayerAttack.Current.ChangeAttackColor(PlayerAttack.Current.CurrentColor);
        }
        //blue
        if (Input.GetKeyDown("3"))
        {
            PlayerAttack.Current.CurrentColor = PlayerAttack.Current.ColorBlue;
            PlayerAttack.Current.ChangeAttackColor(PlayerAttack.Current.CurrentColor);
        }

        //walking
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplier = Mathf.Lerp(speedMultiplier, 0.5f, 0.05f);
        }

        else
            speedMultiplier = Mathf.Lerp(speedMultiplier, 1, 0.05f);

        //reloading
        if (Input.GetKeyDown("r"))
        {
            if (MagicBallOrbit.Current.FiredBallList.Count > 0 && IsNotBusy)
            {
                PlayerAttack.Current.ActivateReload();
            }
        }

        //sound
        if (hInput != 0 || vInput != 0)
            PlayFootsteps();
    }

    void Talk(TalkInteraction talkTarget)
    {
        TalkTarget = talkTarget;
        TalkTarget.InitialiseConversation();

        DisableHud();
        EnableConversationUI();
        DisableCameraMovement();

        animator.SetBool("Talking", true);
        isTalking = true;
    }

    public void StopTalking()
    {
        TalkTarget = null;

        EnableHud();
        DisableConversationUI();
        EnableCameraMovement();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator.SetBool("Talking", false);
        isTalking = false;
    }

    void EnableHud()
    {
        CanvasGroup canvas = HudCanvas.GetComponent<CanvasGroup>();
        canvas.alpha = 1;
    }

    void DisableHud()
    {
        CanvasGroup canvas = HudCanvas.GetComponent<CanvasGroup>();
        canvas.alpha = 0;
    }

    void EnableConversationUI()
    {
        CanvasGroup canvas = ConversationCanvas.GetComponent<CanvasGroup>();
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    void DisableConversationUI()
    {
        CanvasGroup canvas = ConversationCanvas.GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void DisableCameraMovement()
    {
        GO_CameraRig.GetComponent<FreeLookCam>().enabled = false;
    }

    void EnableCameraMovement()
    {
        GO_CameraRig.GetComponent<FreeLookCam>().enabled = true;
    }

    void PlayFootsteps()
    {
        if (playerCharController.isGrounded)
        {
            if (timeSinceLastFootstep > FootstepFrequency / speedMultiplier)
            {
                FootstepAudio.Play();
                timeSinceLastFootstep = 0;
            }
            else
                timeSinceLastFootstep += Time.deltaTime;
        }
    }

    public void PlayAttackSound(AudioClip sound)
    {
        AttackAudio.clip = sound;
        AttackAudio.Play();

        fadeIn = true;
    }

    public void StopAttackSound()
    {
        fadeIn = false;
    }

    void HandleSound()
    {
        if (fadeIn)
            AttackAudio.volume += Time.deltaTime * 2f;
        else
            AttackAudio.volume -= Time.deltaTime * 2f;
    }
}
