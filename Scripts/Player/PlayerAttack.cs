using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Current;

    [Header("GameObjects and Prefabs")]
    public GameObject HandLocation;
    public Text CurrentAmmo;
    public GameObject AttackOrbPrefab;

    [Header("Colors")]
    public Color ColorBlue = Color.blue;
    public Color ColorRed = Color.red;
    public Color ColorGreen = Color.green;
    [HideInInspector]
    public Color CurrentColor;

    [Header("Attack Skills")]
    public GameObject GO_AoeAttack;
    public GameObject GO_BeamAttack;

    [Header("Sound")]
    public AudioClip AttackSound;

    private Animator animator;
    private bool playerNotBusy;
    private PlayerSkills playerSkills;

    public PlayerAttack()
    {
        Current = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        playerSkills = PlayerSkills.Current;
        CurrentColor = ColorRed;

        ChangeAttackColor(CurrentColor);
    }

    void Update()
    {
        if (!PlayerController.Current.InCombat) //do nothing if player is not in combat mode
            return;

        playerNotBusy = PlayerController.Current.IsNotBusy;

        //if click to attack
        if (Input.GetMouseButton(0) && playerNotBusy)
        {
            //balls to attack with
            if (MagicBallOrbit.Current.BallList.Count > 0)
            {
                PlayerController.Current.IsAttacking = true;
                animator.SetTrigger("Attack");
                //Invoke("Attack", 0.9f);
                Activate();
            }
            else if (Input.GetMouseButtonDown(0) && playerNotBusy)
            {
                ActivateReload();
                //animator.SetTrigger("Reload");
                //Invoke("Reload", 1.15f);
            }
        }

        //if press 'Q' to AOE attack
        if (Input.GetKeyDown("q") && playerNotBusy)
        {
            if (playerSkills.IsAoeReady)
            {
                PlayerController.Current.IsAttacking = true;
                animator.SetTrigger("AttackAOE");
                //Invoke("AttackAOE", 1.78f);
                AttackAOE();
            }
        }

        //if press 'E' to beam attack
        if (Input.GetKeyDown("e") && playerNotBusy)
        {
            if (playerSkills.IsBeamReady)
            {
                PlayerController.Current.IsAttacking = true;
                animator.SetTrigger("AttackBeam");
                //Invoke("AttackBeam", 1.01f);
                AttackBeam();
            }
        }
    }

    public void Activate()
    {
        Invoke("Fire", 0.9f);
    }

    void Fire()
    {
        if (MagicBallOrbit.Current.BallList.Count > 0)
        {
            RaycastHit hit;
            Vector3 targetVector;
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Friendly")) //cant shoot friendly character
                {
                    PlayerController.Current.IsAttacking = false;
                    return;
                }

                targetVector = hit.point;
            }
            else
                targetVector = ray.GetPoint(100);

            //have to assign it something thought it should always be overwritten
            GameObject currentClosestGO = MagicBallOrbit.Current.BallList[0];
            float currentClosestMagnitude = 99999;

            foreach (GameObject go in MagicBallOrbit.Current.BallList)
            {
                if (Vector3.Magnitude(HandLocation.transform.position - go.transform.position) < currentClosestMagnitude)
                {
                    currentClosestGO = go;
                    currentClosestMagnitude = Vector3.Magnitude(HandLocation.transform.position - go.transform.position);
                }
            }

            MagicBallOrbit.Current.BallList.Remove(currentClosestGO);
            MagicBallOrbit.Current.FiredBallList.Add(currentClosestGO);
            currentClosestGO.SetActive(false);

            CurrentAmmo.text = MagicBallOrbit.Current.BallList.Count.ToString(); //update ammo count

            GameObject newBall = (GameObject)Instantiate(AttackOrbPrefab, currentClosestGO.transform.position, Quaternion.identity);
            newBall.transform.LookAt(targetVector);
            newBall.GetComponent<Rigidbody>().AddForce(newBall.transform.forward * 1000); //= ray.direction * 10;

            newBall.GetComponent<AbstractAttack>().DamageType = GetAttackType(CurrentColor);

            PlayerController.Current.PlayAttackSound(AttackSound);
            Invoke("Stop", 1);
        }
    }

    void Stop()
    {
        PlayerController.Current.IsAttacking = false;
    }

    void AttackAOE()
    {
        PlayerController.Current.IsAttacking = true;

        GO_AoeAttack.GetComponent<AbstractAttack>().DamageType = PlayerAttack.GetAttackType(CurrentColor);
        GO_AoeAttack.GetComponent<AoeAttack>().Activate();
    }

    void AttackBeam()
    {
        PlayerController.Current.IsAttacking = true;

        GO_BeamAttack.GetComponent<AbstractAttack>().DamageType = PlayerAttack.GetAttackType(CurrentColor);
        GO_BeamAttack.GetComponent<BeamAttack>().Activate();
    }

    public void ActivateReload()
    {
        animator.SetTrigger("Reload");
        Invoke("Reload", 1.15f);
        PlayerController.Current.IsAttacking = true;
    }

    void Reload()
    {
        if (MagicBallOrbit.Current.FiredBallList.Count > 0) //if a ball has been fired
        {
            foreach (GameObject go in MagicBallOrbit.Current.FiredBallList)
            {
                go.SetActive(true);
                MagicBallOrbit.Current.BallList.Add(go);
            }

            MagicBallOrbit.Current.FiredBallList.Clear();

            CurrentAmmo.text = MagicBallOrbit.Current.BallList.Count.ToString();

            Invoke("StopReload", 1);
        }
    }

    void StopReload()
    {
        PlayerController.Current.IsAttacking = false;
    }

    public void ChangeAttackColor(Color color)
    {
        //change orbs color
        foreach (GameObject go in MagicBallOrbit.Current.BallList)
            go.GetComponent<ParticleSystem>().startColor = color;
        foreach (GameObject go in MagicBallOrbit.Current.FiredBallList)
            go.GetComponent<ParticleSystem>().startColor = color;
        AttackOrbPrefab.GetComponent<ParticleSystem>().startColor = color;
        AttackOrbPrefab.GetComponent<MagicBallAttack>().Explosion.GetComponent<ParticleSystem>().startColor = color;

        //change skills color
        GO_AoeAttack.GetComponent<AoeAttack>().SetColor(color);
        GO_BeamAttack.GetComponent<BeamAttack>().SetColor(color);

        //change player related colors
        PlayerController.Current.PlayerLight.color = color;
        PlayerController.Current.AirParticles.startColor = color;
    }

    public static int GetAttackType(Color color)
    {
        int attackType = 0;
        if (color == PlayerAttack.Current.ColorRed)
            attackType = 2;
        else if (color == PlayerAttack.Current.ColorGreen)
            attackType = 3;
        else if (color == PlayerAttack.Current.ColorBlue)
            attackType = 4;

        return attackType;
    }

    public void TriggerInterrupt()
    {
        GO_AoeAttack.GetComponent<AoeAttack>().Interrupt();
        GO_BeamAttack.GetComponent<BeamAttack>().Interrupt();
        CancelInvoke("Fire");
        CancelInvoke("Reload");
    }
}
