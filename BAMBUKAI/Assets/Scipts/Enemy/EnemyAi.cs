using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
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
    private Rigidbody2D rb;

    public EnemyHitbox hitbox;
    private EnemyHealth health;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // prevents tipping
        currentState = EnemyState.Patrol;
        patrolTarget = pointB;
        anim = GetComponent<Animator>();
        health = GetComponent<EnemyHealth>();

    }

    void Update()
    {

        if (health != null && health.isDying)
        {
            rb.linearVelocity = Vector2.zero;   // stop movement

            anim.SetFloat("Speed", 0);
            anim.SetBool("IsChasing", false);
            anim.SetBool("IsAttacking", false);
            return; // <-- STOPS ALL AI
        }
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

        // Stop horizontal movement when switching states
        if (newState == EnemyState.Idle || newState == EnemyState.Attack)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    // -----------------------
    // STATE METHODS
    // -----------------------

    void Idle()
    {
        anim.SetFloat("Speed", 0);
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

        Vector2 direction = (patrolTarget.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

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

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);

        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        FlipSprite(player.position);
    }

    void Attack()
    {
        anim.SetBool("IsChasing", false);
        anim.SetBool("IsAttacking", true);
        anim.SetFloat("Speed", 0);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // stop horizontal movement during attack


        FlipSprite(player.position);
    }

    // This method should be called via an **Animation Event** in the attack animation
    public void PerformAttackHit()
    {
        if (hitbox != null)
            hitbox.DoHit();

        Debug.Log("Enemy hits the player!");
    }

    public void EndAttack()
    {
        anim.SetBool("IsAttacking", false);

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        if (distToPlayer < detectionRange)
            ChangeState(EnemyState.Chase);
        else
            ChangeState(EnemyState.Patrol);
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
