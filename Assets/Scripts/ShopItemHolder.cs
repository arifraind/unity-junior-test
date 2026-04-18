using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemHolder : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image actionButtonImage;
    [SerializeField] private Button actionButton;

    private ShopItemData itemData;
    private ShopController shopController;

    public void Setup(ShopItemData data, ShopController controller)
    {
        itemData = data;
        shopController = controller;

        iconImage.sprite = itemData.icon;
        nameText.text = itemData.itemName;
        descriptionText.text = itemData.description;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnActionPressed);

        Refresh();
    }

    public void Refresh()
    {
        if (itemData == null || shopController == null)
            return;

        if (!shopController.IsOwned(itemData))
        {
            actionButtonImage.sprite = itemData.buySprite;
        }
        else if (shopController.IsEquipped(itemData))
        {
            actionButtonImage.sprite = itemData.equippedSprite;
        }
        else
        {
            actionButtonImage.sprite = itemData.equipSprite;
        }
    }

    private void OnActionPressed()
    {
        if (itemData == null || shopController == null)
            return;

        shopController.OnItemButtonPressed(itemData);
        Refresh();
    }
}