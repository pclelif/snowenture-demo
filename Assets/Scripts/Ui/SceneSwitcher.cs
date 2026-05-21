using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene transitions when the player enters a trigger zone.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private int sceneBuildIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
    }
}