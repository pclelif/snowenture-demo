using UnityEngine;

/// <summary>
/// Handles scarecrow reactions:
/// - Plays idle animation on start
/// - Triggers hit animation when hit by snowball
/// </summary>
public class Scarecrow : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Components
    // ─────────────────────────────────────────────

    private Animator animator;

    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    [Header("Settings")]
    [SerializeField] private string snowballTag = "Snowball";

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("Idle");
        }
    }

    // ─────────────────────────────────────────────
    // Hit Reaction
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(snowballTag))
            return;

        PlayHit();
    }

    public void PlayHit()
    {
        if (animator == null)
            return;

        animator.Play("Scarecrow", 0, 0f);
    }
}