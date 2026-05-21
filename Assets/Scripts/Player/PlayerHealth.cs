using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all player health functionality:
/// - Damage & healing
/// - Invincibility frames
/// - Stun handling
/// - Hit feedback
/// - Death state
/// - UI synchronization
/// </summary>

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerHealth : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Health Settings
    // ─────────────────────────────────────────────

    [Header("Health Settings")]

    [SerializeField, Min(1)]
    public int maxHealth = 120;

    [SerializeField]
    private int currentHealth;

    [SerializeField, Min(0f)]
    private float invincibilityDuration = 0.5f;

    // ─────────────────────────────────────────────
    //  References
    // ─────────────────────────────────────────────

    [Header("References")]

    [SerializeField]
    private PlayerHealthUI healthUI;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // ─────────────────────────────────────────────
    //  Hit Flash Settings
    // ─────────────────────────────────────────────

    [Header("Hit Flash")]

    [SerializeField]
    private Color hitFlashColor = new Color(0.6f, 0.02f, 0.02f);

    [SerializeField, Min(0f)]
    private float flashSpeed = 8f;

    // ─────────────────────────────────────────────
    //  Runtime State
    // ─────────────────────────────────────────────

    public static bool IsDead { get; private set; }

    public bool IsInvincible => isInvincible;

    public int CurrentHealth => currentHealth;

    public int MaxHealth => maxHealth;

    private bool isInvincible;
    private float lastHitTime;

    // ─────────────────────────────────────────────
    //  Cached Components
    // ─────────────────────────────────────────────

    private Collider2D playerCollider;
    private SnowmanMovement movement;

    // ─────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CacheComponents();
        IsDead = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        RefreshHealthUI();
    }

    // ─────────────────────────────────────────────
    //  Initialization
    // ─────────────────────────────────────────────

    private void CacheComponents()
    {
        playerCollider = GetComponent<Collider2D>();
        movement = GetComponent<SnowmanMovement>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (spriteRenderer == null)
            Debug.LogWarning("[PlayerHealth] Missing SpriteRenderer.");

        if (healthUI == null)
            Debug.LogWarning("[PlayerHealth] Missing Health UI reference.");
#endif
    }

    // ─────────────────────────────────────────────
    //  Damage System
    // ─────────────────────────────────────────────

    public void TakeDamage(float damage)
    {
        TakeDamage(Mathf.RoundToInt(damage));
    }

    public void TakeDamage(int damage)
    {
        if (!CanTakeDamage())
            return;

        ApplyDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        healthUI?.DamageFlash();
        StartCoroutine(HitRoutine());
    }

    public void TakeStunDamage(int damage, float duration)
    {
        if (!CanTakeDamage())
            return;

        ApplyDamage(damage);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        ApplyStun(duration);
        StartCoroutine(HitRoutine());
    }

    private bool CanTakeDamage()
    {
        return !IsDead && !isInvincible;
    }

    private void ApplyDamage(int damage)
    {
        lastHitTime = Time.time;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(
            $"<color=red>[PLAYER DAMAGE]</color> " +
            $"Damage: {damage} | " +
            $"HP After: {currentHealth - damage}"
        );
#endif

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        RefreshHealthUI();
    }

    // ─────────────────────────────────────────────
    //  Healing
    // ─────────────────────────────────────────────

    public void Heal(int amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        RefreshHealthUI();
    }

    // ─────────────────────────────────────────────
    //  Upgrade System
    // ─────────────────────────────────────────────

    public void ApplyUpgrade(int amount)
    {
        if (amount <= 0) return;

        int target = currentHealth + amount;

        IncreaseMaxHealthIfNeeded(target);

        currentHealth = Mathf.Clamp(target, 0, maxHealth);
        RefreshHealthUI();
    }

    private void IncreaseMaxHealthIfNeeded(int target)
    {
        const int maxCap = 200;
        const int step = 40;

        while (maxHealth < target && maxHealth < maxCap)
            maxHealth += step;

        maxHealth = Mathf.Min(maxHealth, maxCap);
    }

    // ─────────────────────────────────────────────
    //  Status
    // ─────────────────────────────────────────────

    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    // ─────────────────────────────────────────────
    //  UI
    // ─────────────────────────────────────────────

    private void RefreshHealthUI()
    {
        healthUI?.UpdateHealth(currentHealth, maxHealth);
    }

    // ─────────────────────────────────────────────
    //  Invincibility / Hit Effect
    // ─────────────────────────────────────────────

    private IEnumerator HitRoutine()
    {
        isInvincible = true;

        if (spriteRenderer != null)
            yield return StartCoroutine(FlashRoutine());
        else
            yield return new WaitForSeconds(invincibilityDuration);

        isInvincible = false;
    }

    private IEnumerator FlashRoutine()
    {
        Color original = spriteRenderer.color;
        float t = 0f;

        while (t < invincibilityDuration)
        {
            t += Time.deltaTime;

            float lerp = Mathf.PingPong(t * flashSpeed, 1f);
            spriteRenderer.color = Color.Lerp(original, hitFlashColor, lerp);

            yield return null;
        }

        spriteRenderer.color = original;
    }

    // ─────────────────────────────────────────────
    //  Stun
    // ─────────────────────────────────────────────

    private void ApplyStun(float duration)
    {
        if (movement == null) return;
        StartCoroutine(movement.StunRoutine(duration));
    }

    // ─────────────────────────────────────────────
    //  Death
    // ─────────────────────────────────────────────

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (playerCollider != null)
            playerCollider.enabled = false;

        RefreshHealthUI();

        Destroy(gameObject, 0.2f);
    }

    // ─────────────────────────────────────────────
    //  External Safe API (CRITICAL FIX)
    // ─────────────────────────────────────────────

    /// <summary>
    /// Use this instead of directly modifying invincibility.
    /// </summary>
    public void ForceResetInvincibility()
    {
        isInvincible = false;
    }
}