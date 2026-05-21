using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Controls the attack/ultimate charge UI bar, including glow, flash and infinity mode states.
/// </summary>
public class AttackBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 12f;

    [Header("Colors")]
    [SerializeField] private Color emptyColor = new Color(0.02f, 0.02f, 0.02f, 1f);
    [SerializeField] private Color fillColor = new Color(1f, 0.1f, 0.1f, 1f);
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private Color infinityColor = new Color(0f, 0.8f, 1f, 1f);

    private float targetFill;
    private bool isGlowing;
    private bool ultiReady;
    private bool isInfinityMode;

    private Coroutine glowRoutine;

    private void Awake()
    {
        targetFill = 0f;

        if (fillImage != null)
        {
            fillImage.fillAmount = 0f;
            fillImage.color = emptyColor;
        }
    }

    private void Update()
    {
        if (fillImage == null) return;

        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFill, Time.deltaTime * smoothSpeed);

        if (!isGlowing)
        {
            fillImage.color = isInfinityMode
                ? infinityColor
                : Color.Lerp(emptyColor, fillColor, Mathf.Sqrt(fillImage.fillAmount));
        }
    }

    public void UpdateAttack(float current, float max)
    {
        if (isInfinityMode) return;

        float ratio = Mathf.Clamp01(current / max);
        targetFill = ratio;

        if (ratio >= 1f && !ultiReady)
        {
            ultiReady = true;
            StartGlow();
        }
        else if (ratio < 1f)
        {
            ultiReady = false;
            StopGlow();
        }
    }

    public void SetInfinityMode(bool active)
    {
        isInfinityMode = active;

        if (active)
        {
            targetFill = 1f;
            StopGlow();
            if (fillImage != null) fillImage.color = infinityColor;
        }
        else
        {
            ResetBar();
        }
    }

    public void ResetBar()
    {
        ultiReady = false;
        targetFill = 0f;
        StopGlow();
    }

    public void Flash()
    {
        if (!gameObject.activeInHierarchy || fillImage == null) return;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        fillImage.color = flashColor;
        yield return new WaitForSeconds(0.1f);

        fillImage.color = isInfinityMode ? infinityColor : fillColor;
    }

    private void StartGlow()
    {
        if (isGlowing) return;

        isGlowing = true;
        glowRoutine = StartCoroutine(GlowRoutine());
    }

    private void StopGlow()
    {
        isGlowing = false;

        if (glowRoutine != null)
        {
            StopCoroutine(glowRoutine);
            glowRoutine = null;
        }
    }

    private IEnumerator GlowRoutine()
    {
        while (isGlowing)
        {
            if (fillImage == null) yield break;

            fillImage.color = flashColor;
            yield return new WaitForSeconds(0.3f);

            fillImage.color = fillColor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}