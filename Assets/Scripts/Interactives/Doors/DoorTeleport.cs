using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

/// <summary>
/// Handles door-based teleportation:
/// - Key-based access control
/// - Player teleportation
/// - Camera switching & confiner update
/// - Optional door animation
/// </summary>
public class DoorTeleport : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Lock Settings
    // ─────────────────────────────────────────────

    [Header("Lock Settings")]
    [Tooltip("Does this door require a key?")]
    public bool isLocked = true;

    public Key.KeyType requiredKey;

    // ─────────────────────────────────────────────
    // Teleport Target
    // ─────────────────────────────────────────────

    [Header("Teleport Target")]
    public Transform destinationPoint;

    // ─────────────────────────────────────────────
    // Camera Settings
    // ─────────────────────────────────────────────

    [Header("Camera Settings")]
    public CinemachineCamera targetCamera;
    public Collider2D targetRoomBounds;

    // ─────────────────────────────────────────────
    // Visual (Optional)
    // ─────────────────────────────────────────────

    [Header("Visual (Optional)")]
    public Animator doorAnimator;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private static bool isTeleporting = false;
    private bool isDoorOpened = false;

    // ─────────────────────────────────────────────
    // Unity Events
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isTeleporting)
            return;

        SnowmanMovement player = other.GetComponent<SnowmanMovement>();
        if (player == null) return;

        if (isDoorOpened)
        {
            StartCoroutine(TeleportRoutine(other.gameObject, true));
            return;
        }

        if (HasAccess(player))
        {
            StartCoroutine(TeleportRoutine(other.gameObject, false));
        }
    }

    // ─────────────────────────────────────────────
    // Access Check
    // ─────────────────────────────────────────────

    private bool HasAccess(SnowmanMovement player)
    {
        if (!isLocked)
            return true;

        return requiredKey switch
        {
            Key.KeyType.SolOda => player.hasSolOdaKey,
            Key.KeyType.UstOda => player.hasUstOdaKey,
            Key.KeyType.Boss   => player.hasBossKey,
            Key.KeyType.SonOda => player.hasSonOdaKey,
            _ => false
        };
    }

    // ─────────────────────────────────────────────
    // Teleport Routine
    // ─────────────────────────────────────────────

    private IEnumerator TeleportRoutine(GameObject playerObj, bool alreadyOpen)
    {
        isTeleporting = true;

        if (!alreadyOpen)
        {
            OpenDoor();
            yield return new WaitForSeconds(0.5f);

            isDoorOpened = true;
            isLocked = false;
        }

        playerObj.transform.position = destinationPoint.position;

        UpdateCamera();

        yield return new WaitForSeconds(0.3f);

        isTeleporting = false;
    }

    // ─────────────────────────────────────────────
    // Door Logic
    // ─────────────────────────────────────────────

    private void OpenDoor()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
        }
    }

    // ─────────────────────────────────────────────
    // Camera Logic
    // ─────────────────────────────────────────────

    private void UpdateCamera()
    {
        if (targetCamera == null)
            return;

        foreach (var cam in FindObjectsOfType<CinemachineCamera>())
        {
            cam.Priority = 10;
        }

        targetCamera.Priority = 20;

        var confiner = targetCamera.GetComponent<CinemachineConfiner2D>();
        if (confiner != null && targetRoomBounds != null)
        {
            confiner.BoundingShape2D = targetRoomBounds;
            confiner.InvalidateCache();
        }
    }
}