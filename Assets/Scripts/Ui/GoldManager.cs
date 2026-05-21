using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Manages player's gold currency, UI updates, and global gold events.
/// </summary>
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Header("Gold Settings")]
    [SerializeField] private int gold = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI goldDisplayText;

    public int Gold => gold;

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGoldUI();
        OnGoldChanged?.Invoke(gold);
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        gold += amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(gold);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        gold -= amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    private void UpdateGoldUI()
    {
        if (goldDisplayText != null)
            goldDisplayText.text = $"Gold: {gold}";
    }
}