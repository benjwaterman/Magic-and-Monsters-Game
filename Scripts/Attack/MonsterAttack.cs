using UnityEngine;
using System.Collections;

public class MonsterAttack : AbstractAttack
{
    private GameObject thisEnemy;
    void OnTriggerEnter(Collider coll)
    {
        //if attack triggers hits player
        if (coll.gameObject.CompareTag("Player") && !PlayerHealth.isDead)
        {
            coll.gameObject.GetComponent<PlayerHealth>().TakeDamage(Damage);
            thisEnemy.GetComponent<EnemyController>().DisableAttack(); //so cant damage multiple times in same frame
        }
    }

    public void SetEnemy(GameObject enemy)
    {
        thisEnemy = enemy;
    }

    public void SetDamage(int damage)
    {
        Damage = damage;
    }
}
