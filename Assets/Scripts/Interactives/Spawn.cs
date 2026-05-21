using UnityEngine;

/// <summary>
/// Handles simple trigger-based teleportation:
/// - Teleports player to a target position on enter
/// </summary>
public class Spawn : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Teleport Settings
    // ─────────────────────────────────────────────

    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportTarget;

    // ─────────────────────────────────────────────
    // Trigger
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (teleportTarget == null)
            return;

        other.transform.position = teleportTarget.position;
    }
}