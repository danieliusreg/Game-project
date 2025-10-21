using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;       // tuščias objektas po kojom
    [SerializeField] float groundRadius = 0.12f;  // apskritimo spindulys
    [SerializeField] LayerMask groundMask;        // Layer'iai, kurie laikomi žeme

    Rigidbody2D rb;
    bool facingRight = true;
    float lastAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Horizontalus judėjimas (A/D arba rodyklės)
        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // Apvertimas pagal judėjimo kryptį
        if (x < 0 && !facingRight) Flip();
        else if (x > 0 && facingRight) Flip();

        // Šuolis (Space) – tik kai ant žemės
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        
    }

    bool IsGrounded()
    {
        // patikrina apskritimu po kojom
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    // kad matytum „groundCheck“ gizmo scenoje
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
