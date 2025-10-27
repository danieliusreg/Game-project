using System.Collections;
using UnityEngine;
using TMPro;

public class FridgaNPC : MonoBehaviour
{
    [Header("Interakcija")]
    public Transform player;
    public float talkRange = 2.5f;

    [Header("Animacija")]
    public Animator anim;

    [Header("Dialogo UI")]
    public GameObject dialogPanel;
    public TMP_Text dialogText;

    [Header("Dialogo turinys")]
    [TextArea(3, 6)] public string[] dialogLines;

    [Header("Greitis")]
    [Range(0.01f, 0.1f)] public float typingSpeed = 0.03f;

    [Header("Shop nuoroda")]
    [SerializeField] ShopUI shop;   // <- PRISKIRK INSPECTOR'iuje

    bool isTalking = false;
    Coroutine typingCoroutine;

    void Start()
    {
        dialogPanel.SetActive(false);

        // Atsarginis variantas, jei pamirštum priskirti Inspector'iuje:
        if (shop == null)
        {
        #if UNITY_2023_1_OR_NEWER
                    shop = FindFirstObjectByType<ShopUI>(FindObjectsInactive.Exclude);
        #else
                    shop = FindObjectOfType<ShopUI>(); // randa tik aktyvius
        #endif
        }
    }

    void Update()
    {
        if (player && Vector2.Distance(transform.position, player.position) > talkRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking) StartDialog();
            else
            {
                if (typingCoroutine != null) { StopCoroutine(typingCoroutine); typingCoroutine = null; }
                EndDialog();
            }
        }
    }

    void StartDialog()
    {
        isTalking = true;
        int i = Random.Range(0, dialogLines.Length);
        anim.SetBool("IsTalking", true);
        dialogPanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeText(dialogLines[i]));
    }

    void EndDialog()
    {
        isTalking = false;
        anim.SetBool("IsTalking", false);
        dialogPanel.SetActive(false);

        // Atidarom shop (jei nuoroda yra)
        if (shop != null) shop.OpenShop();
        else Debug.LogWarning("ShopUI nerastas: ar UI_Manager aktyvus? ar ShopUI priskirtas?");
    }

    IEnumerator TypeText(string line)
    {
        dialogText.text = "";
        foreach (char c in line) { dialogText.text += c; yield return new WaitForSeconds(typingSpeed); }
        typingCoroutine = null;
    }
}
