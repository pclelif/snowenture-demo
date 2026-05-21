using UnityEngine;

/// <summary>
/// Deals periodic damage to the player when they stay inside the enemy's breath hitbox.
/// Only active during YetiBreathState.
/// </summary>
public class EnemyDamageDealer : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // References
    // ─────────────────────────────────────────────

    private Enemy enemy;

    // ─────────────────────────────────────────────
    // Damage Settings
    // ─────────────────────────────────────────────

    [Header("Damage Settings")]
    [SerializeField] private float damage = 20f;

    [SerializeField] private float damageInterval = 1.0f;

    private float nextDamageTime;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsPlayer(collision))
            return;

        if (!IsInBreathState())
            return;

        TryApplyDamage(collision.gameObject);
    }

    // ─────────────────────────────────────────────
    // Damage Logic
    // ─────────────────────────────────────────────

    private void TryApplyDamage(GameObject target)
    {
        if (Time.time < nextDamageTime)
            return;

        ApplyDamage(target);

        nextDamageTime = Time.time + damageInterval;
    }

    private void ApplyDamage(GameObject target)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(Mathf.RoundToInt(damage));
        }
    }

    // ─────────────────────────────────────────────
    // State Checks
    // ─────────────────────────────────────────────

    private bool IsPlayer(Collider2D col)
    {
        return col.CompareTag("Player");
    }

    private bool IsInBreathState()
    {
        return enemy != null &&
               enemy.StateMachine != null &&
               enemy.StateMachine.CurrentState is YetiBreathState;
    }
}