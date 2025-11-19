using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover")]
    public float hoverScale = 1.06f;  // labai minimalus padidėjimas
    public float ease = 12f;          // per kiek greitai „prilenda“

    [Header("Navigation")]
    public string sceneToLoad;
    public MapUI map;                 // nutempk savo MapUI (Zemelapis) čia

    Vector3 baseScale, targetScale;

    void Awake()
    {
        baseScale = transform.localScale;
        targetScale = baseScale;
    }

    void OnEnable() => transform.localScale = baseScale;

    void Update()
    {
        // sklandus priartinimas net kai Time.timeScale==0
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * ease);
    }

    public void OnPointerEnter(PointerEventData e) => targetScale = baseScale * hoverScale;
    public void OnPointerExit (PointerEventData e) => targetScale = baseScale;

    public void OnPointerClick(PointerEventData e)
    {
        // uždaryk map'ą ir grąžink laiką, tada keisk sceną
        if (map != null) map.Close();
        Time.timeScale = 1f; // labai svarbu, kad naujoje scenoje nebūtų „pauzės“
        SceneManager.LoadScene(sceneToLoad);
    }
}
