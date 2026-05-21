using UnityEngine;

/// <summary>
/// Handles permanent attack power upgrade interaction for the player.
/// </summary>
public class PowerUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private float powerIncreaseAmount = 30f;

    private PlayerAttack playerAttack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerAttack attack))
        {
            playerAttack = attack;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerAttack != null && other.GetComponent<PlayerAttack>() == playerAttack)
        {
            playerAttack = null;
        }
    }

    public bool TryUpgrade()
    {
        if (playerAttack == null) return false;

        playerAttack.ApplyPermanentUpgrade(powerIncreaseAmount);
        return true;
    }
}