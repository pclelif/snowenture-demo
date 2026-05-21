using UnityEngine;
using TMPro;

/// <summary>
/// Handles simple interaction zone UI:
/// - Shows prompt when player enters range
/// - Displays cost information
/// - Hides UI when player exits
/// </summary>
public class Interaction : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;

    // ─────────────────────────────────────────────
    // Interaction Data
    // ─────────────────────────────────────────────

    [Header("Cost")]
    [SerializeField] private int cost = -200;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

     private bool isPlayerInRange;
    
    private SnowmanMovement playerMovement;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        HidePrompt();
    }

    // ─────────────────────────────────────────────
    // Trigger Detection
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = true;
        playerMovement = other.GetComponent<SnowmanMovement>();

        ShowPrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = false;
        playerMovement = null;

        HidePrompt();
    }

    // ─────────────────────────────────────────────
    // UI Logic
    // ─────────────────────────────────────────────

    private void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(true);

        if (promptText != null)
            promptText.text = $"F\n{cost} Gold";
    }

    private void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}