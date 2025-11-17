using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public EnemyState currentState;

    public Transform pointA;
    public Transform pointB;
    private Transform patrolTarget;

    public Transform player;

    [Header("Stats")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float idleDuration = 2f;
    public float detectionRange = 6f;
    public float attackRange = 1.2f;

    private float idleTimer;

    private Animator anim;

    void Start()
    {
        currentState = EnemyState.Patrol;
        patrolTarget = pointB;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Global transitions
        if (distToPlayer < attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distToPlayer < detectionRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else if (currentState == EnemyState.Chase && distToPlayer > detectionRange)
        {
            ChangeState(EnemyState.Patrol);
        }

        // State behavior
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        if (newState == EnemyState.Idle)
            idleTimer = idleDuration;
    }

    // -----------------------
    // STATE METHODS
    // -----------------------

    void Idle()
    {
        anim.SetFloat("Speed",0);
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    void Patrol()
    {
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", false);

        float move = patrolSpeed;
        anim.SetFloat("Speed", Mathf.Abs(move));

        transform.position = Vector2.MoveTowards(
            transform.position,
            patrolTarget.position,
            patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolTarget.position) < 0.2f)
        {
            ChangeState(EnemyState.Idle);

            patrolTarget = (patrolTarget == pointA) ? pointB : pointA;
        }

        FlipSprite(patrolTarget.position);
    }

    void Chase()
    {
        anim.SetBool("IsChasing", true);
        anim.SetBool("IsAttacking", false);
        
        float move = chaseSpeed;
        anim.SetFloat("Speed", Mathf .Abs(move));

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime);

        FlipSprite(player.position);
    }

    void Attack()
    {
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", true);
        anim.SetFloat("Speed", 0);
        // Attack animation/event here
        Debug.Log("Enemy attacks!");

        // Keep facing the player  
        FlipSprite(player.position);
    }

    // -----------------------
    // HELPERS
    // -----------------------
    void FlipSprite(Vector2 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmos()
    {   
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
