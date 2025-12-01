using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.12f;
    [SerializeField] LayerMask groundMask;

    [Header("Attack")]
    public Collider2D attackHitBox;

    [Header("Shield")]
    public bool canBlock = true;    // jei kada nors norėsi išjungti skydą

    Rigidbody2D rb;
    Animator anim;

    bool facingRight = true;
    float inputX;

    bool isAttacking = false;
    bool isPreparingJump = false;
    public bool isBlocking = false;        // <-- nauja būsena

    RigidbodyConstraints2D defaultConstraints;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        defaultConstraints = rb.constraints;
    }

    void Update()
    {
        bool grounded = IsGrounded();

        // --- SKYDAS ---

        bool canUseShield = canBlock && !isAttacking && !isPreparingJump;

        if (canUseShield)
        {
            // dešinys pelės mygtukas (Input sistemoje – "Fire2")
            if (Input.GetButtonDown("Fire2"))
            {
                isBlocking = true;
                anim.SetBool("Blocking", true);
            }

            if (Input.GetButtonUp("Fire2"))
            {
                isBlocking = false;
                anim.SetBool("Blocking", false);
            }
        }

        // --- ATAKA --- (negalima kai laikomas skydas)
        if (!isAttacking && !isPreparingJump && !isBlocking && grounded && Input.GetButtonDown("Fire1"))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }

        // --- ŠUOLIO PRADŽIA (pritūpimas) --- (irgi ne per skydą)
        if (!isAttacking && !isPreparingJump && !isBlocking && grounded && Input.GetButtonDown("Jump"))
        {
            isPreparingJump = true;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            anim.SetTrigger("Jump");               // Pasokimas_pradeti
        }

        // Horizontalus inputas – išjungiam jei ataka, pritūpimas ARBA skydas
        if (isAttacking || isPreparingJump || isBlocking)
            inputX = 0f;
        else
            inputX = Input.GetAxisRaw("Horizontal");

        // Flip
        if (inputX < 0f && !facingRight) Flip();
        else if (inputX > 0f && facingRight) Flip();

        // Animator parametrai
        float speedAbs = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", speedAbs);
        anim.SetBool("Grounded", grounded);
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Įprastas judėjimas (kai ne pritūpimas ir ne skydas, inputX jau bus 0)
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

    // ==== Animation Events ====

    // kviečiama iš Pasokimas_pradeti paskutinio frame (event "DoJump")
    public void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isPreparingJump = false;
    }

    public void AttackStart()
    {
        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

        if (attackHitBox != null)
            attackHitBox.GetComponent<SwordHitbox>().DoHit();
    }

    public void AttackEnd()
    {
        isAttacking = false;
        rb.constraints = defaultConstraints;
    }
}
