using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int coins = 100;

    public bool Spend(int amount)
    {
        if (coins < amount) return false;
        coins -= amount;
        return true;
    }

    public void AddCoins(int amount) => coins += amount;
}
