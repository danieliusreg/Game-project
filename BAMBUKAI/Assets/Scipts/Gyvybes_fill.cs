using UnityEngine;
using UnityEngine.UI;

public class HealthFillUI : MonoBehaviour
{
    public PlayerHealth player;   // nutempk Player
    public Image fillImage;       // nutempk "Fill" (raudonas Image)

    void Start()
    {
        if (!player) player = FindObjectOfType<PlayerHealth>();
        Set(player.currentHealth / (float)player.maxHealth);
        player.OnHealthPctChanged += Set;
    }

    void OnDestroy()
    {
        if (player) player.OnHealthPctChanged -= Set;
    }

    void Set(float pct) => fillImage.fillAmount = pct;
}
