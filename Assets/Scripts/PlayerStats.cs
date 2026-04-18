using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public Sprite CurrentJumpBarSprite { get; private set; }

    [SerializeField] private Sprite defaultJumpBarSprite;

    [Header("Base Stats")]
    [SerializeField] private float baseJumpForce = 10f;

    public float BaseJumpForce => baseJumpForce;
    public float CurrentJumpForce { get; private set; }
    public int ExtraAirJumps { get; private set; }
    public bool HasShield { get; private set; }
    public bool HasDoubleCoins { get; private set; }
    public bool HasChargeEffect { get; private set; }

    public float JumpForceMultiplier => CurrentJumpForce / baseJumpForce;

    private void Awake()
    {
        ResetToBase();
    }

    public void ResetToBase()
    {
        CurrentJumpForce = baseJumpForce;
        ExtraAirJumps = 0;
        HasShield = false;
        HasDoubleCoins = false;
        HasChargeEffect = false;
        CurrentJumpBarSprite = defaultJumpBarSprite;
    }

    public void ApplyEquippedItems(ItemData equippedCharacter, ItemData equippedSpring)
    {
        ResetToBase();

        ApplySingleItem(equippedCharacter);
        ApplySingleItem(equippedSpring);
    }

    private void ApplySingleItem(ItemData item)
    {
        if (item == null)
            return;

        CurrentJumpForce += baseJumpForce * item.jumpForcePercentBonus;
        ExtraAirJumps += item.extraAirJumps;

        if (item.givesShield)
            HasShield = true;

        if (item.givesDoubleCoins)
            HasDoubleCoins = true;

        if (item.enablesChargeEffect)
            HasChargeEffect = true;

        if (item.jumpBarFillSprite != null)
            CurrentJumpBarSprite = item.jumpBarFillSprite;
    }

    public bool HasSignificantForceBonus(float threshold = 1.1f)
    {
        return JumpForceMultiplier > threshold;
    }
}