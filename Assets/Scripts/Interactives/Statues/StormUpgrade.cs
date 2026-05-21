using UnityEngine;

/// <summary>
/// Handles movement speed upgrade interaction for the player.
/// </summary>
public class StormUpgrade : MonoBehaviour
{
    [Header("Upgrade Settings")]
    [SerializeField] private float speedIncreaseAmount = 1f;

    private SnowmanMovement playerMovement;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out SnowmanMovement movement))
        {
            playerMovement = movement;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerMovement != null && other.GetComponent<SnowmanMovement>() == playerMovement)
        {
            playerMovement = null;
        }
    }

    public bool TryUpgrade()
    {
        if (playerMovement == null) return false;

        playerMovement.IncreaseSpeed(speedIncreaseAmount);
        return true;
    }
}