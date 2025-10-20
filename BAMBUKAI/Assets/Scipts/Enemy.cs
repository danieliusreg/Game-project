using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;         // žaid?jo nuoroda
    public float detectionRadius = 5f; // matymo spindulys
    public float speed = 2f;           // jud?jimo greitis

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        // Apskai?iuojame atstum? iki žaid?jo
        float distance = Vector2.Distance(transform.position, player.position);

        // Jei žaid?jas spindulyje – vejasi, kitaip laukia
        isChasing = distance <= detectionRadius;

        if (isChasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            movement = direction;
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        // Judiname prieš?, jei vejasi
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // Parodo spindul? Scene vaizde
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
