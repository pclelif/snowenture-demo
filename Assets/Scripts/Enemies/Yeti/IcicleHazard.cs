using UnityEngine;
using System.Collections;

/// <summary>
/// Icicle hazard that warns the player, falls, then deals damage + stun.
/// Handles telegraphed attack sequence with visual feedback.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class IcicleHazard : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Visual References
    // ─────────────────────────────────────────────

    [Header("Visual References")]
    [SerializeField] private SpriteRenderer warningCircle;
    [SerializeField] private SpriteRenderer icicleSprite;

    // ─────────────────────────────────────────────
    // Timing
    // ─────────────────────────────────────────────

    [Header("Timing Settings")]
    [SerializeField] private float warningDuration = 1.5f;
    [SerializeField] private float dropDuration = 0.1f;
    [SerializeField] private float fadeOutDuration = 0.8f;
    [SerializeField] private float lingerDuration = 0.3f;

    // ─────────────────────────────────────────────
    // Scale Settings
    // ─────────────────────────────────────────────

    [Header("Size Settings")]
    [SerializeField] private float maxWarningSize = 3.0f;
    [SerializeField] private float maxIcicleSize = 1.0f;

    // ─────────────────────────────────────────────
    // Combat
    // ─────────────────────────────────────────────

    [Header("Damage Settings")]
    [SerializeField] private int damage = 15;
    [SerializeField] private float stunDuration = 1.5f;

    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Collider2D damageCollider;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        damageCollider = GetComponent<Collider2D>();

        if (damageCollider != null)
            damageCollider.enabled = false;
    }

    private void Start()
    {
        StartCoroutine(AttackSequence());
    }

    private void LateUpdate()
    {
        UpdateSortingOrder();
    }

    // ─────────────────────────────────────────────
    // Attack Sequence
    // ─────────────────────────────────────────────

    private IEnumerator AttackSequence()
    {
        SetState(warning: true, icicle: false);

        yield return AnimateWarningCircle();

        SetState(warning: false, icicle: true);

        StartCoroutine(FadeOutWarningCircle());

        yield return AnimateIcicleDrop();

        EnableDamage();

        float waitTime = Mathf.Max(lingerDuration, fadeOutDuration);
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // Animation Steps
    // ─────────────────────────────────────────────

    private IEnumerator AnimateWarningCircle()
    {
        float t = 0f;

        while (t < warningDuration)
        {
            t += Time.deltaTime;

            float scale = Mathf.Lerp(0.1f, maxWarningSize, t / warningDuration);

            if (warningCircle != null)
                warningCircle.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }

    private IEnumerator AnimateIcicleDrop()
    {
        float t = 0f;

        while (t < dropDuration)
        {
            t += Time.deltaTime;

            float scale = Mathf.Lerp(0.1f, maxIcicleSize, t / dropDuration);

            if (icicleSprite != null)
                icicleSprite.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }

    private IEnumerator FadeOutWarningCircle()
    {
        if (warningCircle == null)
            yield break;

        Color startColor = warningCircle.color;

        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;

            float alpha = Mathf.Lerp(startColor.a, 0f, t / fadeOutDuration);

            warningCircle.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                alpha
            );

            yield return null;
        }

        warningCircle.enabled = false;
    }

    // ─────────────────────────────────────────────
    // Damage
    // ─────────────────────────────────────────────

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        SnowmanMovement move = collision.GetComponent<SnowmanMovement>();

        if (move == null || !move.canMove)
            return;

        PlayerHealth health = collision.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeStunDamage(damage, stunDuration);
        }
    }

    private void EnableDamage()
    {
        if (damageCollider != null)
            damageCollider.enabled = true;
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private void SetState(bool warning, bool icicle)
    {
        if (warningCircle != null)
            warningCircle.enabled = warning;

        if (icicleSprite != null)
            icicleSprite.enabled = icicle;
    }

    private void UpdateSortingOrder()
    {
        int baseOrder = Mathf.RoundToInt(-transform.position.y * 100);

        if (warningCircle != null)
            warningCircle.sortingOrder = baseOrder - 1;

        if (icicleSprite != null)
            icicleSprite.sortingOrder = baseOrder + 5;
    }
}