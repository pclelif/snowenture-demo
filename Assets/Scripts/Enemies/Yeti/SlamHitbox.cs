using UnityEngine;

/// <summary>
/// Handles Yeti slam hitbox damage and stun application.
/// Applies damage only when player is in a valid movement state.
/// </summary>
public sealed class SlamHitbox : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Damage Settings
    // ─────────────────────────────────────────────

    [Header("Slam Damage Settings")]

    [SerializeField, Min(0)]
    private int damage = 20;

    [SerializeField, Min(0f)]
    private float stunDuration = 1.5f;

    // ─────────────────────────────────────────────
    // Trigger
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        ApplyDamage(collision);
    }

    // ─────────────────────────────────────────────
    // Core Logic
    // ─────────────────────────────────────────────

    private void ApplyDamage(Collider2D target)
    {
        SnowmanMovement movement =
            target.GetComponent<SnowmanMovement>();

        if (movement == null || !movement.canMove)
        {
            return;
        }

        PlayerHealth health =
            target.GetComponent<PlayerHealth>();

        if (health == null)
        {
            return;
        }

        health.TakeStunDamage(damage, stunDuration);
    }
}