using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles player health UI visualization:
/// - Heart display
/// - Heart state updates
/// - Damage flash feedback
/// </summary>
public sealed class PlayerHealthUI : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Heart References
    // ─────────────────────────────────────────────

    [Header("Heart References")]

    [SerializeField]
    private Image[] heartImages;

    [SerializeField]
    private Sprite fullHeartSprite;

    [SerializeField]
    private Sprite halfHeartSprite;

    [SerializeField]
    private Sprite emptyHeartSprite;

    // ─────────────────────────────────────────────
    //  Heart Settings
    // ─────────────────────────────────────────────

    [Header("Heart Settings")]

    [SerializeField, Min(1f)]
    private float healthPerHeart = 40f;

    // ─────────────────────────────────────────────
    //  Damage Flash Settings
    // ─────────────────────────────────────────────

    [Header("Damage Flash")]

    [SerializeField]
    private Color flashColor =
        new Color(1f, 1f, 1f, 0.5f);

    [SerializeField, Min(0f)]
    private float flashDuration = 0.1f;

    // ─────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD

        ValidateReferences();

#endif
    }

    // ─────────────────────────────────────────────
    //  Health Update
    // ─────────────────────────────────────────────

    public void UpdateHealth(
        float currentHealth,
        float maxHealth)
    {
        int visibleHeartCount =
            Mathf.CeilToInt(
                maxHealth / healthPerHeart
            );

        for (int i = 0; i < heartImages.Length; i++)
        {
            Image heartImage = heartImages[i];

            if (heartImage == null)
            {
                continue;
            }

            bool shouldBeVisible =
                i < visibleHeartCount;

            heartImage.gameObject.SetActive(
                shouldBeVisible
            );

            if (!shouldBeVisible)
            {
                continue;
            }

            UpdateHeartSprite(
                heartImage,
                currentHealth,
                i
            );
        }
    }

    private void UpdateHeartSprite(
        Image heartImage,
        float currentHealth,
        int heartIndex)
    {
        float heartStartValue =
            heartIndex * healthPerHeart;

        float remainingHealth =
            currentHealth - heartStartValue;

        if (remainingHealth >= healthPerHeart)
        {
            heartImage.sprite = fullHeartSprite;
        }
        else if (remainingHealth >=
                 healthPerHeart * 0.5f)
        {
            heartImage.sprite = halfHeartSprite;
        }
        else
        {
            heartImage.sprite = emptyHeartSprite;
        }
    }

    // ─────────────────────────────────────────────
    //  Damage Flash
    // ─────────────────────────────────────────────

    public void DamageFlash()
    {
        StopAllCoroutines();

        StartCoroutine(PlayDamageFlash());
    }

    private IEnumerator PlayDamageFlash()
    {
        SetHeartColors(flashColor);

        yield return new WaitForSeconds(
            flashDuration
        );

        SetHeartColors(Color.white);
    }

    private void SetHeartColors(Color color)
    {
        foreach (Image heartImage in heartImages)
        {
            if (heartImage == null)
            {
                continue;
            }

            heartImage.color = color;
        }
    }

    // ─────────────────────────────────────────────
    //  Validation
    // ─────────────────────────────────────────────

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void ValidateReferences()
    {
        if (heartImages == null ||
            heartImages.Length == 0)
        {
            Debug.LogWarning(
                "[PlayerHealthUI] No heart images assigned.",
                this
            );
        }

        if (fullHeartSprite == null)
        {
            Debug.LogWarning(
                "[PlayerHealthUI] Full heart sprite is missing.",
                this
            );
        }

        if (halfHeartSprite == null)
        {
            Debug.LogWarning(
                "[PlayerHealthUI] Half heart sprite is missing.",
                this
            );
        }

        if (emptyHeartSprite == null)
        {
            Debug.LogWarning(
                "[PlayerHealthUI] Empty heart sprite is missing.",
                this
            );
        }
    }

#endif
}