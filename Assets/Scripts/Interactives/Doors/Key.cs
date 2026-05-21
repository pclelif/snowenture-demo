using UnityEngine;

/// <summary>
/// Handles key pickup logic:
/// - Assigns key to player inventory flags
/// - Destroys key object after pickup
/// </summary>
public class Key : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Key Types
    // ─────────────────────────────────────────────

    public enum KeyType
    {
        SolOda,
        UstOda,
        Boss,
        SonOda
    }

    [Header("Key Type")]
    public KeyType myKeyType;

    // ─────────────────────────────────────────────
    // Trigger
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SnowmanMovement player = other.GetComponent<SnowmanMovement>();
        if (player == null)
            return;

        GiveKey(player);

        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // Key Logic
    // ─────────────────────────────────────────────

    private void GiveKey(SnowmanMovement player)
    {
        switch (myKeyType)
        {
            case KeyType.SolOda:
                player.hasSolOdaKey = true;
                break;

            case KeyType.UstOda:
                player.hasUstOdaKey = true;
                break;

            case KeyType.Boss:
                player.hasBossKey = true;
                break;

            case KeyType.SonOda:
                player.hasSonOdaKey = true;
                break;
        }
    }
}