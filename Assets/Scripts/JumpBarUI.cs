using UnityEngine;
using UnityEngine.UI;

public class JumpBarUI : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Image fillImage;

    private void Start()
    {
        RefreshBarSprite();
    }

    public void RefreshBarSprite()
    {
        if (playerStats == null || fillImage == null)
            return;

        fillImage.sprite = playerStats.CurrentJumpBarSprite;
    }
}