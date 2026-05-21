using UnityEngine;

/// <summary>
/// Defines a training zone that temporarily modifies player attack behavior.
/// </summary>
public class TrainingZone : MonoBehaviour
{
    [Header("Runtime Reference")]
    private PlayerAttack playerRef;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent(out PlayerAttack playerAttack))
        {
            playerRef = playerAttack;
            playerRef.EnterTrainingZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerRef != null)
        {
            playerRef.ExitTrainingZone();
            playerRef = null;
        }
    }
}