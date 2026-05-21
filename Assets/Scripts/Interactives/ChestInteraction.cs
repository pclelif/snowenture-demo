using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Handles chest interaction UI and gold reward:
/// - Shows/hides prompt UI
/// - Gives gold on interaction
/// - Supports keyboard + gamepad input
/// </summary>
public class ChestInteraction : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;

    // ─────────────────────────────────────────────
    // Reward
    // ─────────────────────────────────────────────

    [Header("Reward")]
    [SerializeField] private int goldAmount = 200;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isPlayerInRange;
    private bool isOpened;

    // (Unused currently, kept for future extension)
    private SnowmanMovement playerMovement;

    // ─────────────────────────────────────────────
    // Input
    // ─────────────────────────────────────────────

    private InputAction interactAction;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        interactAction = new InputAction("Interact", InputActionType.Button);

        interactAction.AddBinding("<Keyboard>/f");
        interactAction.AddBinding("<Gamepad>/buttonSouth");
    }

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        if (!isPlayerInRange || isOpened)
            return;

        if (interactAction.WasPressedThisFrame())
        {
            OpenChest();
        }
    }

    // ─────────────────────────────────────────────
    // Chest Logic
    // ─────────────────────────────────────────────

    private void OpenChest()
    {
        isOpened = true;

        GiveReward();
        HidePrompt();
    }

    private void GiveReward()
    {
        if (GoldManager.Instance != null)
        {
            GoldManager.Instance.AddGold(goldAmount);
        }
    }

    // ─────────────────────────────────────────────
    // UI Logic
    // ─────────────────────────────────────────────

    private void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(true);

        if (promptText != null)
            promptText.text = $"F / A\n+{goldAmount} Gold";
    }

    private void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    // ─────────────────────────────────────────────
    // Trigger Detection
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInRange = true;
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
}