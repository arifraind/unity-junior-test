using UnityEngine;

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Item")]
public class ShopItemData : ItemData
{
    [Header("Shop Info")]
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite icon;
    public Sprite buySprite;
    public Sprite equipSprite;
    public Sprite equippedSprite;

    [Header("Cost")]
    public int price;
}