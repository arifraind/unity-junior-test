using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text coinsText;

    public int CurrentCoins
    {
        get
        {
            if (PlayerDataManager.Instance == null)
                return 0;

            return PlayerDataManager.Instance.CurrentCoins;
        }
    }

    private void Start()
    {
        UpdateCoinsUI();
    }

    [SerializeField] private PlayerStats playerStats;

    public void AddCoins(int amount)
    {
        if (PlayerDataManager.Instance == null)
            return;

        int finalAmount = amount;

        if (playerStats != null && playerStats.HasDoubleCoins)
            finalAmount *= 2;

        PlayerDataManager.Instance.AddCoins(finalAmount);
        UpdateCoinsUI();
    }

    public bool SpendCoins(int amount)
    {
        if (PlayerDataManager.Instance == null)
            return false;

        bool success = PlayerDataManager.Instance.SpendCoins(amount);
        UpdateCoinsUI();
        return success;
    }

    public void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = CurrentCoins.ToString();
    }
}