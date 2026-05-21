using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles chest interaction:
/// - Opens chest with input
/// - Plays animation
/// - Spawns key with impulse
/// </summary>
public class Chest : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // References
    // ─────────────────────────────────────────────

    [Header("Chest Settings")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Animator chestAnimator;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isPlayerNear = false;
    private bool isOpened = false;

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

        interactAction.AddBinding("<Keyboard>/r");
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

    private void Update()
    {
        if (!isPlayerNear || isOpened)
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

        PlayOpenAnimation();
        SpawnKey();
    }

    private void PlayOpenAnimation()
    {
        if (chestAnimator != null)
            chestAnimator.SetTrigger("Open");
    }

    private void SpawnKey()
    {
        if (keyPrefab == null || spawnPoint == null)
            return;

        GameObject key = Instantiate(keyPrefab, spawnPoint.position, Quaternion.identity);

        Rigidbody2D rb = key.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 pushDirection = new Vector2(
                Random.Range(-0.5f, 0.5f),
                1.5f
            );

            rb.AddForce(pushDirection * 4f, ForceMode2D.Impulse);
        }
    }

    // ─────────────────────────────────────────────
    // Trigger Detection
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}