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
    RigidbodyConstraints2D defaultConstraints;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        defaultConstraints = rb.constraints;                     // pasiimam pradinius
    }

    void Update()
    {
        // Startuojam ataką tik jei dabar ne ataka
        if (!isAttacking && Input.GetButtonDown("Fire1"))
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
        }

        // Neskaitom horizontalios krypties kai atakuojam
        inputX = isAttacking ? 0f : Input.GetAxisRaw("Horizontal");

        // Šuolis tik jei ne ataka
        if (!isAttacking && Input.GetButtonDown("Jump") && IsGrounded())
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        // Apvertimas
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
            // Sustabdom slydimą
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        // Įprastas judėjimas
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

    // ==== Animation Events (kviečiami iš "Atakuoti" klipo) ====

    // Kviečiam pirmame klipo frame
    public void AttackStart()
    {
        isAttacking = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        // visiškai užrakina slinkimą per ataką:
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    // Kviečiam paskutiniame klipo frame
    public void AttackEnd()
    {
        isAttacking = false;
        rb.constraints = defaultConstraints;  // grąžinam kaip buvo
    }
}
