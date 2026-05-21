using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

/// <summary>
/// Handles statue interaction system:
/// - Player upgrade interaction (health/power/storm)
/// - Gold cost spending
/// - Light + shake feedback effect
/// </summary>
public class StatueInteract : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // UI
    // ─────────────────────────────────────────────

    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMP_Text promptText;

    // ─────────────────────────────────────────────
    // Visual Effects
    // ─────────────────────────────────────────────

    [Header("Visual")]
    [SerializeField] private Light2D statueLight;

    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    [Header("Settings")]
    [SerializeField] private int cost = 100;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isPlayerInRange;
    private Vector3 originalPosition;

    // ─────────────────────────────────────────────
    // Upgrades
    // ─────────────────────────────────────────────

    private HealthUpgrade healthUpgrade;
    private PowerUpgrade powerUpgrade;
    private StormUpgrade stormUpgrade;

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

    private void OnEnable() => interactAction.Enable();
    private void OnDisable() => interactAction.Disable();

    private void Start()
    {
        originalPosition = transform.localPosition;

        healthUpgrade = GetComponent<HealthUpgrade>();
        powerUpgrade = GetComponent<PowerUpgrade>();
        stormUpgrade = GetComponent<StormUpgrade>();

        if (statueLight != null)
            statueLight.intensity = 0f;

        HidePrompt();
    }

    private void Update()
    {
        if (!isPlayerInRange)
            return;

        if (interactAction.WasPressedThisFrame())
        {
            TryInteract();
        }
    }

    // ─────────────────────────────────────────────
    // Interaction Logic
    // ─────────────────────────────────────────────

    private void TryInteract()
    {
        if (GoldManager.Instance == null)
            return;

        if (!TryApplyUpgrade())
            return;

        if (GoldManager.Instance.TrySpendGold(cost))
        {
            StartCoroutine(ActivateStatueEffect());
        }
    }

    private bool TryApplyUpgrade()
    {
        if (healthUpgrade != null)
            return healthUpgrade.TryUpgrade();

        if (powerUpgrade != null)
            return powerUpgrade.TryUpgrade();

        if (stormUpgrade != null)
            return stormUpgrade.TryUpgrade();

        return true;
    }

    // ─────────────────────────────────────────────
    // Feedback Effect
    // ─────────────────────────────────────────────

    private IEnumerator ActivateStatueEffect()
    {
        if (statueLight != null)
            statueLight.intensity = 2f;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition =
                originalPosition + (Vector3)Random.insideUnitCircle * 0.04f;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;

        if (statueLight != null)
            statueLight.intensity = 0f;
    }

    // ─────────────────────────────────────────────
    // Trigger
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
        HidePrompt();
    }

    // ─────────────────────────────────────────────
    // UI Helpers
    // ─────────────────────────────────────────────

    private void ShowPrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(true);

        if (promptText != null)
            promptText.text = $"F / A\n{cost} Gold";
    }

    private void HidePrompt()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }
}