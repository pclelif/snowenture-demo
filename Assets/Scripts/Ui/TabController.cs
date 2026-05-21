using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls in-game tab menu system (pages, tabs, pause/resume).
/// </summary>
public class TabController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Image[] tabImages;

    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuCanvas != null && menuCanvas.activeSelf)
                ResumeGame();
            else
                OpenMenu();
        }
    }

    public void OpenMenu()
    {
        if (menuCanvas == null) return;

        menuCanvas.SetActive(true);
        Time.timeScale = 0f;

        ActivateTab(0);
    }

    public void ResumeGame()
    {
        if (menuCanvas == null) return;

        menuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ActivateTab(int tabNo)
    {
        if (pages == null || tabImages == null) return;

        for (int i = 0; i < tabImages.Length; i++)
        {
            if (tabImages[i] != null)
                tabImages[i].color = inactiveColor;
        }

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(false);
        }

        if (tabNo >= 0 && tabNo < tabImages.Length && tabImages[tabNo] != null)
            tabImages[tabNo].color = activeColor;

        if (tabNo >= 0 && tabNo < pages.Length && pages[tabNo] != null)
            pages[tabNo].SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}