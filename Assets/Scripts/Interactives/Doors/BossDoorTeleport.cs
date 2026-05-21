using UnityEngine;
using System.Collections;

/// <summary>
/// Handles boss door teleport sequence:
/// - Plays door open/close animation
/// - Teleports player to boss arena
/// - Enables physical barrier after transition
/// </summary>
public class BossDoorTeleport : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // References
    // ─────────────────────────────────────────────

    [Header("References")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private Transform teleportTarget;
    [SerializeField] private BoxCollider2D doorCollider;

    // ─────────────────────────────────────────────
    // Timing
    // ─────────────────────────────────────────────

    [Header("Timing")]
    [SerializeField] private float delayBeforeTeleport = 0.4f;
    [SerializeField] private float delayBeforeClosing = 0.6f;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isSequenceRunning = false;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (doorCollider != null)
            doorCollider.enabled = false;
    }

    // ─────────────────────────────────────────────
    // Trigger
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isSequenceRunning || !other.CompareTag("Player"))
            return;

        isSequenceRunning = true;
        StartCoroutine(TeleportSequence(other.gameObject));
    }

    // ─────────────────────────────────────────────
    // Sequence
    // ─────────────────────────────────────────────

    private IEnumerator TeleportSequence(GameObject player)
    {
        PlayOpenAnimation();

        yield return new WaitForSeconds(delayBeforeTeleport);

        TeleportPlayer(player);

        yield return new WaitForSeconds(delayBeforeClosing);

        PlayCloseAnimation();

        EnableDoorCollider();
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private void PlayOpenAnimation()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");
    }

    private void PlayCloseAnimation()
    {
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Close");
    }

    private void TeleportPlayer(GameObject player)
    {
        if (player != null && teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
        }
    }

    private void EnableDoorCollider()
    {
        if (doorCollider != null)
            doorCollider.enabled = true;
    }
}