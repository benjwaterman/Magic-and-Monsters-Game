using UnityEngine;
using System.Collections;

public class AoeAttack : AbstractAttack
{
    public AudioClip AoeAttackSound;

    private SphereCollider aoeCollider;
    private ParticleSystem particleSys;
    private Collider attackTrigger;

    void Start()
    {
        aoeCollider = GetComponent<SphereCollider>();

        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        particleSys = GetComponent<ParticleSystem>();

        attackTrigger = GetComponent<Collider>();
        attackTrigger.enabled = false;
    }

    void Update()
    {
        if (aoeCollider.enabled)
        {
            transform.localScale += new Vector3(1, 1, 1) * 30 * Time.deltaTime;
        }
        else
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = coll.gameObject.GetComponent<EnemyController>();
            if (DamageType == enemy.enemyDamageType) //if this damage type is same as enemy's
            {
                enemy.TakeDamage(Damage);
            }
        }
    }

    public void Activate()
    {
        Invoke("Fire", 1.78f);
    }

    void Fire()
    {
        PlayerSkills.Current.ActivateAoe();
        PlayerController.Current.PlayAttackSound(AoeAttackSound);
        particleSys.Play();
        Invoke("Stop", 0.6f);

        attackTrigger.enabled = true;
        MagicBallOrbit.Current.ScaleTo(5);
    }

    void Stop()
    {
        attackTrigger.enabled = false;
        MagicBallOrbit.Current.ScaleTo(1);
        PlayerController.Current.IsAttacking = false;
    }

    public void SetColor(Color color)
    {
        particleSys.startColor = color;
    }

    public void Interrupt()
    {
        CancelInvoke("Fire");

        Stop();
    }
}
