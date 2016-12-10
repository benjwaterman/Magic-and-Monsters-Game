using UnityEngine;
using System.Collections;

public class MagicBallAttack : AbstractAttack
{
    public GameObject Explosion;

    void Start()
    {
        Destroy(this.gameObject, 5);
        Physics.IgnoreCollision(GetComponent<Collider>(), PlayerController.Current.GetComponent<Collider>());
    }

    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = coll.gameObject.GetComponent<EnemyController>();
            if (DamageType == enemy.enemyDamageType) //if this damage type is same as enemy's
            {
                enemy.TakeDamage(Damage);
            }
        }

        if (!coll.transform.CompareTag("Player"))
        {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
