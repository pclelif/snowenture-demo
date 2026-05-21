using UnityEngine;
using TMPro;

/// <summary>
/// UI listener for displaying player's gold value.
/// Subscribes to GoldManager events and updates UI automatically.
/// </summary>
public class GoldUI : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TMP_Text goldText;

    private void OnEnable()
    {
        if (GoldManager.Instance == null) return;

        GoldManager.Instance.OnGoldChanged += UpdateText;
        UpdateText(GoldManager.Instance.Gold);
    }

    private void OnDisable()
    {
        if (GoldManager.Instance == null) return;

        GoldManager.Instance.OnGoldChanged -= UpdateText;
    }

    private void UpdateText(int newGold)
    {
        if (goldText == null) return;

        goldText.text = $"Gold: {newGold}";
    }
}