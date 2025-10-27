using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Root")]
    public GameObject shopPanel;

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text item1Text;
    public TMP_Text item2Text;
    public TMP_Text item3Text;
    public Button buy1Btn;
    public Button buy2Btn;
    public Button buy3Btn;
    public Button closeBtn;

    [Header("Prekės")]
    public string item1Name = "Pirmoji prekė";
    public string item2Name = "Antroji prekė";
    public string item3Name = "Trečioji prekė";
    public int item1Price = 50;
    public int item2Price = 75;
    public int item3Price = 120;

    PlayerInventory inv;

    void Awake()
    {
        inv = FindObjectOfType<PlayerInventory>();
        shopPanel.SetActive(false);

        // pririšam mygtukus
        buy1Btn.onClick.AddListener(() => TryBuy(1));
        buy2Btn.onClick.AddListener(() => TryBuy(2));
        buy3Btn.onClick.AddListener(() => TryBuy(3));
        closeBtn.onClick.AddListener(CloseShop);
    }

    public void OpenShop()
    {
        RefreshTexts();
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    void RefreshTexts()
    {
        if (!inv) inv = FindObjectOfType<PlayerInventory>();

        coinsText.text = $"Monetos: {inv.coins}";
        item1Text.text = $"{item1Name} – {item1Price}";
        item2Text.text = $"{item2Name} – {item2Price}";
        item3Text.text = $"{item3Name} – {item3Price}";
    }

    void TryBuy(int index)
    {
        int price = 0;
        string name = "";
        switch (index)
        {
            case 1: price = item1Price; name = item1Name; break;
            case 2: price = item2Price; name = item2Name; break;
            case 3: price = item3Price; name = item3Name; break;
        }

        if (inv != null && inv.Spend(price))
        {
            // TODO: čia duok daiktą žaidėjui (upgrade, potion, ammo ir t.t.)
            Debug.Log($"Nupirkta: {name} ({price})");
            RefreshTexts();
        }
        else
        {
            Debug.Log("Nepakanka monetų!");
        }
    }
}
