using UnityEngine;

public enum ItemType
{
    Character,
    Spring,
    Item
}

[CreateAssetMenu(menuName = "Shop/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemType itemType;

    [Header("Gameplay")]
    public float jumpForcePercentBonus;
    public int extraAirJumps;
    public bool givesShield;
    public bool givesDoubleCoins;
    public bool enablesChargeEffect;

    [Header("Visuals")]
    public GameObject characterVisualPrefab;
    public GameObject springVisualPrefab;

    [Header("UI")]
    public Sprite jumpBarFillSprite;
}