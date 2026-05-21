using UnityEngine;
using System.Collections;

/// <summary>
/// Controls automatic bottom door behavior:
/// - Plays closing animation on start
/// - Enables collider after closing delay
/// </summary>
public class BottomDoorControl : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    [Header("Door Settings")]
    [SerializeField] private float closingDelay = 0.1f;
    [SerializeField] private float colliderEnableDelay = 0.2f;

    // ─────────────────────────────────────────────
    // Components
    // ─────────────────────────────────────────────

    private Animator animator;
    private BoxCollider2D doorCollider;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        if (doorCollider != null)
            doorCollider.enabled = false;

        StartCoroutine(CloseDoorRoutine());
    }

    // ─────────────────────────────────────────────
    // Door Routine
    // ─────────────────────────────────────────────

    private IEnumerator CloseDoorRoutine()
    {
        yield return new WaitForSeconds(closingDelay);

        if (animator != null)
            animator.SetTrigger("Close");

        yield return new WaitForSeconds(colliderEnableDelay);

        if (doorCollider != null)
            doorCollider.enabled = true;
    }
}