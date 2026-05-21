using UnityEngine;

/// <summary>
/// Handles in-game menu toggle behavior.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject menuCanvas;

    private void Start()
    {
        if (menuCanvas != null)
            menuCanvas.SetActive(false);
    }

    private void Update()
    {
        if (menuCanvas == null) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            menuCanvas.SetActive(!menuCanvas.activeSelf);
        }
    }
}