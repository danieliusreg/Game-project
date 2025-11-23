using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;

    private Animator anim;
    private SpriteRenderer sr;
    public bool isDying = false;
    public GameObject itemDropPrefab;

    private void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }


    public void TakeDamage(int damage)
    {
        if (isDying) return;  // Prevent taking damage while dying

        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            isDying = true;
            anim.SetTrigger("Die");
            StartCoroutine(Die());
            return;
        }
    }

    private IEnumerator Die()
    {
        isDying = true;

        anim.SetTrigger("Die");

        // Wait for the death animation to start (optional)
        yield return null; // ensure animation triggers
        float dieLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(dieLength);

        // Get all sprite renderers in this object and children
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        float fadeDuration = 3f;
        float t = 0f;

        // Store original colors
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;

        // Fade loop
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }

        // Drop the item at the enemy's position
        if (itemDropPrefab != null)
        {
            Instantiate(itemDropPrefab, transform.position, Quaternion.identity);
        }


        Destroy(gameObject);
    }

}
