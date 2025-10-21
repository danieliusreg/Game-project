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
    Animator anim;
    bool facingRight = true;
    float inputX;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // arba GetComponent<Animator>(), jei Animator ant to paties GO
    }

    void Update()
    {
        // Nuskaitom inputą (čia, kad nebūtų lagų su FixedUpdate)
        inputX = Input.GetAxisRaw("Horizontal");

        // Šuolis (tik kai ant žemės)
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Apvertimas pagal judėjimo kryptį
        if (inputX < 0f && !facingRight) Flip();
        else if (inputX > 0f && facingRight) Flip();

        // --- ANIMACIJA ---
        if (anim)
        {
            float speedAbs = Mathf.Abs(rb.linearVelocity.x);
            anim.SetFloat("Speed", speedAbs);          // Idle/Run per „Speed“
            anim.SetBool("Grounded", IsGrounded());    // jei prireiks šuolio/fall animacijom
        }
    }

    void FixedUpdate()
    {
        // Judėjimas atliekamas per FixedUpdate, kad būtų sklandu
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        // Patikrina apskritimu po kojom
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
