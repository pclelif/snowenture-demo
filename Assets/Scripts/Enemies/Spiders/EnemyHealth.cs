using UnityEngine;
using System.Collections;

/// <summary>
/// Manages enemy health system including damage intake, hit flash visual feedback,
/// death handling, and loot dropping mechanics (e.g., gold spawning with physics force).
/// </summary>

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyHealth : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Health
    // ─────────────────────────────────────────────

    [Header("Health Settings")]

    [SerializeField, Min(1f)]
    private float maxHealth = 30f;

    private float currentHealth;

    // ─────────────────────────────────────────────
    // Hit Effect
    // ─────────────────────────────────────────────

    [Header("Hit Effect")]

    [SerializeField, Min(0.01f)]
    private float hitFlashDuration = 0.1f;

    [SerializeField, Min(1)]
    private int flashCount = 3;

    [SerializeField, Range(0f, 1f)]
    private float minAlpha = 0.2f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool isFlashing;

    // ─────────────────────────────────────────────
    // Loot
    // ─────────────────────────────────────────────

    [Header("Loot")]

    [SerializeField]
    private GameObject goldPrefab;

    [SerializeField, Min(0f)]
    private float scatterForce = 3f;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        InitializeHealth();
        CacheComponents();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
    }

    private void CacheComponents()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ─────────────────────────────────────────────
    // Damage
    // ─────────────────────────────────────────────

    public void TakeDamage(float amount)
    {
        if (amount <= 0f)
            return;

        currentHealth -= amount;

        if (!isFlashing)
            StartCoroutine(PlayHitEffect());

        if (currentHealth <= 0f)
            Die();
    }

    // ─────────────────────────────────────────────
    // Hit Flash
    // ─────────────────────────────────────────────

    private IEnumerator PlayHitEffect()
    {
        isFlashing = true;

        Color originalColor = spriteRenderer.color;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(hitFlashDuration * 0.5f);

            spriteRenderer.color = new Color(1f, 1f, 1f, minAlpha);
            yield return new WaitForSeconds(hitFlashDuration * 0.5f);
        }

        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    // ─────────────────────────────────────────────
    // Death
    // ─────────────────────────────────────────────

    private void Die()
    {
        DropGold();
        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // Loot
    // ─────────────────────────────────────────────

    private void DropGold()
    {
        if (goldPrefab == null)
            return;

        GameObject coin =
            Instantiate(goldPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        float angle = Random.Range(0f, 360f);

        Vector2 direction =
            new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

        rb.AddForce(direction * scatterForce, ForceMode2D.Impulse);
    }
}