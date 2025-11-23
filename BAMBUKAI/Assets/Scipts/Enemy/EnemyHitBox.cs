using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage = 1;
    public LayerMask playerMask;

    public Transform hitPoint;
    public Vector2 hitSize = new Vector2(1.2f, 0.8f);

    public void DoHit()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitPoint.position, hitSize, 0f, playerMask);

        foreach (var hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (hitPoint != null)
        {
            Gizmos.DrawWireCube(hitPoint.position, hitSize);
        }
    }
}
