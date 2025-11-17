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

    Rigidbody2D rb;
    Animator anim;

    bool facingRight = true;
    float inputX;

    bool isAttacking = false;
    bool isPreparingJump = false;     // <-- NAUJA: ar vyksta „pritūpimo“ fazė

    RigidbodyConstraints2D defaultConstraints;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        defaultConstraints = rb.constraints;
    }

    void Update()
    {
        // ATAKA – tik ant žemės ir kai ne šuolis
        if (!isAttacking && !isPreparingJump && IsGrounded() && Input.GetButtonDown("Fire1"))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }

        // ŠUOLIO PRADŽIA – pritūpimas
        if (!isAttacking && !isPreparingJump && IsGrounded() && Input.GetButtonDown("Jump"))
        {
            isPreparingJump = true;                               // užrakinam judėjimą
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // nuimam slydimą
            anim.SetTrigger("Jump");                             // pereinam į Pasokimas_pradeti
        }

        // Horizontalus inputas – išjungiam jei ataka ARBA pritūpimas
        inputX = (isAttacking || isPreparingJump)
            ? 0f
            : Input.GetAxisRaw("Horizontal");

        // Flip
        if (inputX < 0f && !facingRight) Flip();
        else if (inputX > 0f && facingRight) Flip();

        // Animator parametrai
        float speedAbs = Mathf.Abs(rb.linearVelocity.x);
        anim.SetFloat("Speed", speedAbs);
        anim.SetBool("Grounded", IsGrounded());
    }

    void FixedUpdate()
    {
        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Įprastas judėjimas – BET tik jei ne pritūpimo fazė,
        // nes inputX jau bus 0, jei isPreparingJump == true
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

    // Kviečiama iš Pasokimas_pradeti paskutinio frame (event "DoJump")
    public void DoJump()
    {
        // Pridedam vertikalų šuolio greitį
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Baigėsi pritūpimo fazė – ore vėl galima judėti
        isPreparingJump = false;
    }

    // Atakos pradžia – kaip buvo
    public void AttackStart()
    {
        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void AttackEnd()
    {
        isAttacking = false;
        rb.constraints = defaultConstraints;
    }
}
