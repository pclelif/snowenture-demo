using UnityEngine;
using System.Collections;

/// <summary>
/// Handles Yeti boss health system:
/// - Damage intake
/// - Visual feedback (flash effect)
/// - Death handling
/// </summary>
public class YetiHealth : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Health Settings
    // ─────────────────────────────────────────────

    [Header("Health Settings")]

    [SerializeField, Min(1f)]
    private float maxHealth = 400f;

    private float currentHealth;

    // ─────────────────────────────────────────────
    // Visuals
    // ─────────────────────────────────────────────

    [Header("Visual Effects")]

    [SerializeField]
    private SpriteRenderer mainBodySprite;

    [SerializeField]
    private float flashDuration = 0.1f;

    private Color originalColor;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        InitializeHealth();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void CacheComponents()
    {
        if (mainBodySprite == null)
        {
            mainBodySprite =
                GetComponentInChildren<SpriteRenderer>();
        }

        if (mainBodySprite != null)
        {
            originalColor = mainBodySprite.color;
        }
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
    }

    // ─────────────────────────────────────────────
    // Damage System
    // ─────────────────────────────────────────────

    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log(
            $"[YETI HEALTH] Damage: {damage} | " +
            $"Remaining HP: {currentHealth}"
        );
#endif

        StartCoroutine(PlayHitFlash());

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // ─────────────────────────────────────────────
    // Visual Feedback
    // ─────────────────────────────────────────────

    private IEnumerator PlayHitFlash()
    {
        if (mainBodySprite == null)
        {
            yield break;
        }

        mainBodySprite.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        mainBodySprite.color = originalColor;
    }

    // ─────────────────────────────────────────────
    // Death
    // ─────────────────────────────────────────────

    private void Die()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Log("[YETI] Boss defeated!");
#endif

        Destroy(gameObject);
    }
}