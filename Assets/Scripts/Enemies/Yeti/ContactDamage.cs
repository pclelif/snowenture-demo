using UnityEngine;

/// <summary>
/// Applies contact damage to the player when collision occurs.
/// Can be configured to only deal damage during attack states.
/// </summary>
public class ContactDamage : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Damage Settings
    // ─────────────────────────────────────────────

    [Header("Contact Damage Settings")]
    [SerializeField] private int contactDamage = 10;

    // ─────────────────────────────────────────────
    // Attack Control
    // ─────────────────────────────────────────────

    [Header("Attack State")]

    [Tooltip("If enabled, damage is only applied when 'isAttacking' is true.")]
    [SerializeField] private bool damageOnlyOnAttack = true;

    [HideInInspector] public bool isAttacking = false;

    // ─────────────────────────────────────────────
    // Unity Callbacks
    // ─────────────────────────────────────────────

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!CanDealDamage())
            return;

        if (!collision.gameObject.CompareTag("Player"))
            return;

        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(contactDamage);
        }
    }

    // ─────────────────────────────────────────────
    // Internal Logic
    // ─────────────────────────────────────────────

    private bool CanDealDamage()
    {
        if (damageOnlyOnAttack && !isAttacking)
            return false;

        return true;
    }
}