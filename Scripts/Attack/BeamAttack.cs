using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeamAttack : AbstractAttack
{
    public AudioClip BeamAttackSound;

    public float RotationSpeed = 100;
    public float DamageFrequency = 0.5f;

    private BoxCollider beamCollider;
    private ParticleSystem[] particleSysArray;
    private List<EnemyController> enemiesInBeam = new List<EnemyController>();
    private List<float> timeInBeam = new List<float>();
    private Vector3 targetVector;
    private bool isAttacking = false;

    void Start()
    {
        particleSysArray = GetComponentsInChildren<ParticleSystem>();
        beamCollider = GetComponent<BoxCollider>();

        Stop();
    }

    void Update()
    {
        if (isAttacking)
        {
            var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
            targetVector = ray.GetPoint(10);
            targetVector.y = Mathf.Clamp(targetVector.y, transform.position.y - 1, transform.position.y + 1);
            transform.LookAt(targetVector);

            transform.Rotate(new Vector3(0, 0, 1), RotationSpeed * Time.deltaTime); //spiral effect
            
            for(int i = 0; i < enemiesInBeam.Count; i++)
            {
                if(timeInBeam[i] >= DamageFrequency)
                {
                    if (DamageType == enemiesInBeam[i].enemyDamageType) //if this damage type is same as enemy's
                    {
                        enemiesInBeam[i].TakeDamage(Damage);
                        timeInBeam[i] = 0;
                    }
                }

                timeInBeam[i] += Time.deltaTime;
            }
        }
    }

    public void Activate()
    {
        Invoke("Fire", 1.01f); //delay to sync with animation
        MagicBallOrbit.Current.RotateTo(new Vector3(90, 0, 0));
        MagicBallOrbit.Current.MoveBy(new Vector3(0, -0.3f, 1.1f));
        MagicBallOrbit.Current.ScaleTo(0.5f);
        MagicBallOrbit.Current.MultiplyOrbitSpeedBy(1.5f);
    }

    void Fire()
    {
        PlayerSkills.Current.ActivateBeam(); //only trigger cooldown after beam has started firing
        PlayerController.Current.PlayAttackSound(BeamAttackSound);

        isAttacking = true;

        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
        targetVector = ray.GetPoint(10);
        targetVector.y = transform.position.y;
        transform.LookAt(targetVector);

        beamCollider.enabled = true;
        foreach (ParticleSystem p in particleSysArray)
        {
            p.playbackSpeed = 1;
            p.Play();
        }

        Invoke("Stop", 1.55f); //1.15 seconds long
    }

    void Stop()
    {
        PlayerController.Current.StopAttackSound();

        isAttacking = false;

        beamCollider.enabled = false;
        enemiesInBeam.Clear();
        timeInBeam.Clear();
        foreach (ParticleSystem p in particleSysArray)
        {
            p.playbackSpeed = 2;
            p.Stop();

            //get current active particles in systems
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[p.maxParticles];
            int numParticlesAlive = p.GetParticles(particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                //particles[i].velocity *= -1f;//-particles[i].velocity; //reverse direction, to come towards source
                particles[i].startLifetime = .5f;
                p.SetParticles(particles, numParticlesAlive);
            }
        }

        MagicBallOrbit.Current.RotateTo(new Vector3(0, 0, 0));
        MagicBallOrbit.Current.MoveBy(new Vector3(0, 0, 0));
        MagicBallOrbit.Current.ScaleTo(1);
        MagicBallOrbit.Current.MultiplyOrbitSpeedBy(1);

        PlayerController.Current.IsAttacking = false;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = coll.gameObject.GetComponent<EnemyController>();
            enemiesInBeam.Add(enemy);
            timeInBeam.Add(0.5f);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = coll.gameObject.GetComponent<EnemyController>();
            timeInBeam.RemoveAt(enemiesInBeam.IndexOf(enemy)); //remove time counter by enemy index
            enemiesInBeam.Remove(enemy);
        }
    }

    public void SetColor(Color color)
    {
        foreach (ParticleSystem p in particleSysArray)
        {
            p.startColor = color;
        }
    }

    public void Interrupt()
    {
        CancelInvoke("Fire");

        Stop();
    }
}
