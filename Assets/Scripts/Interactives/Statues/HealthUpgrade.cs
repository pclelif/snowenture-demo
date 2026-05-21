using UnityEngine;

/// <summary>
/// Handles health upgrade interaction logic for the player.
/// </summary>
public class HealthUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private int healAmount = 40;

    private PlayerHealth playerHealth;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerHealth health))
        {
            playerHealth = health;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerHealth != null && other.GetComponent<PlayerHealth>() == playerHealth)
        {
            playerHealth = null;
        }
    }

    public bool TryUpgrade()
    {
        if (playerHealth == null) return false;

        if (playerHealth.maxHealth >= 200 && playerHealth.IsFullHealth())
            return false;

        playerHealth.ApplyUpgrade(healAmount);
        return true;
    }
}