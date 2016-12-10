using UnityEngine;
using System.Collections;

public abstract class CharacterInfo : MonoBehaviour
{
    public GameObject HealthOrb;

    public string Name;
    public int CurrentHealth;
    public int MaxHealth;
    public bool IsEnemy;

    private EnemyController enemy; 

    public abstract void OnDeath();

    void Start()
    {
        if(IsEnemy = gameObject.CompareTag("Enemy"))
            enemy = GetComponent<EnemyController>();

    }

    public void TakeDamage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth > 0)
        {
            GetComponent<Animator>().SetTrigger("Damage");

            if (IsEnemy)
            {
                enemy.CanAttack = true;
                enemy.DisableAttack();
            }
        }
    }

    public void DropHealth(int amount)
    {
        if (amount == 0)
            return;

        if(GetComponent<EnemyController>())
        {
            GameObject go = (GameObject)Instantiate(HealthOrb, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
            go.GetComponent<HealthPickup>().HealthRestoreAmount = amount;
            go.GetComponent<ParticleSystem>().startColor = GetComponent<EnemyController>().color;
        }
    }
}
