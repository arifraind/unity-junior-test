using UnityEngine;
using UnityEngine.UI;

public class JumpChargeBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpringJumpController jumpController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image fillImage;

    private Sprite lastSprite;

    private void Update()
    {
        if (jumpController == null || playerStats == null || fillImage == null)
            return;

        // Fill amount (same logic as before)
        fillImage.fillAmount = jumpController.CurrentChargePercent;

        // 🔥 Sprite swapping
        if (playerStats.CurrentJumpBarSprite != lastSprite)
        {
            lastSprite = playerStats.CurrentJumpBarSprite;
            fillImage.sprite = lastSprite;
        }
    }
}