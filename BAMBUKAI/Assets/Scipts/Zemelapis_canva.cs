using UnityEngine;

public class MapUI : MonoBehaviour
{
    public GameObject mapPanel;
    public bool pauseOnOpen = true;

    bool isOpen;

    public void Toggle() { if (isOpen) Close(); else Open(); }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) Toggle();
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) Close();
    }


    public void Open()
    {
        isOpen = true;
        if (mapPanel) mapPanel.SetActive(true);
        if (pauseOnOpen) Time.timeScale = 0f;
    }

    public void Close()
    {
        isOpen = false;
        if (mapPanel) mapPanel.SetActive(false);
        if (pauseOnOpen) Time.timeScale = 1f;
    }

    void Start()
    {
        if (mapPanel) mapPanel.SetActive(false);
    }
}
