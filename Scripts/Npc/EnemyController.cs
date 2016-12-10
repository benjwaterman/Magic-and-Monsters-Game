using UnityEngine;
using System.Collections;

public class EnemyController : CharacterInfo
{
    public enum EnemyType
    {
        Light,
        Heavy,
        Caster
    };
    public EnemyType EnemyClass = EnemyType.Light;

    [HideInInspector]
    public float VSpeed;
    [HideInInspector]
    public bool CanAttack = true;
    [HideInInspector]
    public Spawner ParentSpawner;

    [Range(0.5f, 1)]
    public float MovementSpeed = 1;
    //public float CharacterRotateSpeed = 1000;
    public float WalkRadius = 5;
    public float FindPlayerRange = 15;
    public float AttackPlayerRange = 1.5f;
    public int AttackDamage = 25;
    public int HealthAmountOnDeath; //Choose two values to randomise the health drop between, will only be less than upper value
    [Range(0, 100)]
    public int HealthDropChance;
    public GameObject RightHandCollider;
    public GameObject LeftHandCollider;

    private Animator animator;
    private GameObject player;
    private Vector3 lastKnownPlayerPosition;
    [HideInInspector]
    public Color color;
    [HideInInspector]
    public int enemyDamageType;
    private float lookForPlayerDelay = 0.2f;
    private float timeSinceLastCheck = 0f;
    //private float targetSpeed;
    private bool dead = false;
    private Vector3 startPos;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = PlayerController.Current.gameObject;
        lastKnownPlayerPosition = transform.position; //set to self, if not enemy knows where player is
        CurrentHealth = MaxHealth;

        //can return 2,3,4
        int ran = Random.Range(2, 5);
        enemyDamageType = ran;

        switch (enemyDamageType)
        {
            case 2:
                color = PlayerAttack.Current.ColorRed;
                break;
            case 3:
                color = PlayerAttack.Current.ColorGreen;
                break;
            case 4:
                color = PlayerAttack.Current.ColorBlue;
                break;
        }

        GetComponentInChildren<ParticleSystem>().startColor = color;
        GetComponentInChildren<Light>().color = color;

        InitialiseAttackColliders();
        DisableAttack(); //turn off attack colliders

        agent = GetComponent<NavMeshAgent>();
        startPos = transform.position;

        agent.stoppingDistance = AttackPlayerRange; //shouldn't move if next to the player
    }

    void Update()
    {
        if (dead)
            return;

        if (PlayerHealth.isDead) //if player is dead, stop moving and attacking
        {
            agent.Stop();
            animator.SetInteger("Attack", 0);
        }
        animator.SetFloat("VSpeed", agent.velocity.magnitude / 2);

        //if (agent.velocity.magnitude < 0.1f)
        //    animator.SetFloat("VSpeed", 0);

        //else if (agent.velocity.magnitude / 2 > 0.2f && agent.velocity.magnitude / 2 < 0.5f)
        //    animator.SetFloat("VSpeed", 0.5f);

        //else if (agent.velocity.magnitude / 2 >= 0.5f)
        //    animator.SetFloat("VSpeed", agent.velocity.magnitude / 2);

        //else
        //    animator.SetFloat("VSpeed", 0);

        if (timeSinceLastCheck >= lookForPlayerDelay && !PlayerHealth.isDead)
        {
            animator.SetInteger("Attack", 0);
            timeSinceLastCheck = 0;
            float playerDistance = Vector3.Distance(player.transform.position, transform.position);

            if (playerDistance < AttackPlayerRange) //if player is within attack range
            {
                AttackPlayer();
            }

            else if (playerDistance < FindPlayerRange) //if player is within find player range, walk towards them
            {
                MoveTowardsPlayer();
            }

            else //player not in range, move randomly
            {
                MoveRandom();
            }
        }

        //DEAD
        if (CurrentHealth <= 0 && !dead)
        {
            OnDeath();
        }

        timeSinceLastCheck += Time.deltaTime;
    }

    public override void OnDeath()
    {
        dead = true;
        DisableAttack();
        if (ParentSpawner != null)
            ParentSpawner.RemoveEnemy(this.gameObject);
        if(Random.Range(0, 100) <= HealthDropChance)
            DropHealth(HealthAmountOnDeath);
        animator.SetTrigger("Dead");
        
        lookForPlayerDelay = Mathf.Infinity; //stop finding the player
        agent.Stop();
        agent.enabled = false;
    }

    public void EnableAttack()
    {
        if (!dead)
        {
            LeftHandCollider.SetActive(true);
            RightHandCollider.SetActive(true);
        }
    }

    public void DisableAttack()
    {
        LeftHandCollider.SetActive(false);
        RightHandCollider.SetActive(false);
    }

    void InitialiseAttackColliders() //must be called before disabling colliders
    {
        MonsterAttack rightHand = RightHandCollider.AddComponent<MonsterAttack>();
        MonsterAttack leftHand = LeftHandCollider.AddComponent<MonsterAttack>();

        rightHand.SetEnemy(this.gameObject);
        rightHand.SetDamage(AttackDamage);

        leftHand.SetEnemy(this.gameObject);
        leftHand.SetDamage(AttackDamage);
    }

    void MoveRandom()
    {
        if ((transform.position - agent.destination).magnitude < agent.stoppingDistance || !agent.hasPath) //if enemy doesnt have a path, generate a new one
        {
            Vector3 randomDirection = Random.insideUnitSphere * WalkRadius;
            randomDirection += startPos;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, WalkRadius, 1);
            Vector3 finalPosition = navHit.position;
            agent.SetDestination(finalPosition);
        }
    }

    void MoveTowardsPlayer()
    {
        lastKnownPlayerPosition = player.transform.position;
        agent.speed = MovementSpeed * 2;

        agent.SetDestination(lastKnownPlayerPosition);
    }

    void AttackPlayer()
    {
        if (CanAttack && animator.GetCurrentAnimatorStateInfo(0).IsName("IdleWalk"))
        {
            //enemy type determines what kind of attack will be used
            if (EnemyClass == EnemyType.Light)
            {
                Invoke("EnableAttack", 0.08f);
                //Invoke("DisableAttack", 1f);
                animator.SetInteger("Attack", 1);
            }

            else if (EnemyClass == EnemyType.Heavy)
            {
                Invoke("EnableAttack", 1.08f);
                //Invoke("DisableAttack", 2.5f);
                animator.SetInteger("Attack", 2);
            }
            CanAttack = false;
            animator.SetBool("RightHand", !animator.GetBool("RightHand")); //switch hands
        }
        else
            animator.SetInteger("Attack", 0);
    }
}
