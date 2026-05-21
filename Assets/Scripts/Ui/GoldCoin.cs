using UnityEngine;
using System.Collections;

/// <summary>
/// Handles collectible gold coin behavior:
/// - Auto-destruction after lifetime
/// - Blinking warning before disappearing
/// - Player pickup logic
/// </summary>
public class GoldCoin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int goldValue = 50;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float blinkDuration = 1.0f;

    private SpriteRenderer sr;
    private Coroutine lifeRoutine;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearDamping = 5f;

        lifeRoutine = StartCoroutine(SelfDestructRoutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldValue);

        Destroy(gameObject);
    }

    private IEnumerator SelfDestructRoutine()
    {
        float waitTime = Mathf.Max(0f, lifetime - blinkDuration);
        yield return new WaitForSeconds(waitTime);

        float blinkTimer = 0f;
        float blinkSpeed = 0.1f;

        while (blinkTimer < blinkDuration)
        {
            if (sr != null)
                sr.enabled = !sr.enabled;

            blinkTimer += blinkSpeed;
            yield return new WaitForSeconds(blinkSpeed);
        }

        Destroy(gameObject);
    }
}