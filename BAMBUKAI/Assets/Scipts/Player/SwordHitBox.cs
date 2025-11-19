using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    public int damage = 1;
    public LayerMask enemyMask;

    public Transform hitPoint;
    public Vector2 hitSize = new Vector2(1.2f, 0.8f);

    public void DoHit()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitPoint.position, hitSize, 0f, enemyMask);

        foreach (var hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
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
