using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles room completion logic:
/// - Checks remaining enemies in room
/// - Spawns reward when room is cleared
/// </summary>
public class RoomClearSpawner : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Room Settings
    // ─────────────────────────────────────────────

    [Header("Room Settings")]
    [SerializeField] private List<GameObject> roomEnemies = new();

    // ─────────────────────────────────────────────
    // Reward Settings
    // ─────────────────────────────────────────────

    [Header("Reward Settings")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform spawnPoint;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isRoomCleared = false;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Update()
    {
        if (isRoomCleared)
            return;

        CleanupEnemyList();

        if (IsRoomCleared())
        {
            isRoomCleared = true;
            SpawnReward();
        }
    }

    // ─────────────────────────────────────────────
    // Room Logic
    // ─────────────────────────────────────────────

    private void CleanupEnemyList()
    {
        roomEnemies.RemoveAll(enemy => enemy == null);
    }

    private bool IsRoomCleared()
    {
        return roomEnemies.Count == 0;
    }

    // ─────────────────────────────────────────────
    // Reward Logic
    // ─────────────────────────────────────────────

    private void SpawnReward()
    {
        if (rewardPrefab == null || spawnPoint == null)
            return;

        Instantiate(rewardPrefab, spawnPoint.position, Quaternion.identity);

        Debug.Log("Room cleared!");
    }
}