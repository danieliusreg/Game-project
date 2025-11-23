using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public event Action<float> OnHealthPctChanged; // 0..1

    void Awake() => currentHealth = maxHealth;

    void Start()
    {
        TakeDamage(30);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthPctChanged?.Invoke((float)currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthPctChanged?.Invoke((float)currentHealth / maxHealth);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");
        //Destroy(gameObject);
    }
}
