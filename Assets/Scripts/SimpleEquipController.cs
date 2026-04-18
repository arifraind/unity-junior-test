using UnityEngine;

public class SimpleEquipController : MonoBehaviour
{
    [Header("Equipped Items")]
    [SerializeField] private ItemData equippedCharacter;
    [SerializeField] private ItemData equippedSpring;

    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private SpringJumpController springJumpController;

    [Header("Visual Holders In Player")]
    [SerializeField] private Transform springHolder;

    [Header("Character Offset")]
    [SerializeField] private float characterYOffset = 6.7f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private GameObject chargeEffect;

    private GameObject currentCharacterVisual;
    private GameObject currentSpringVisual;

    private void Start()
    {
        LoadFromPlayerData();
    }

    public void EquipItem(ItemData item)
    {
        if (item == null)
        {
            Debug.LogWarning("EquipItem got null item.");
            return;
        }

        if (item.itemType == ItemType.Character)
        {
            equippedCharacter = item;
            ApplyStats();
            RefreshCharacterVisual();
            RefreshShieldVisual();
        }
        else if (item.itemType == ItemType.Spring)
        {
            equippedSpring = item;
            ApplyStats();
            RefreshSpringVisual();
            RefreshCharacterVisual();
            RefreshChargeEffectParent();
        }
    }

    public void ApplyAll()
    {
        ApplyStats();
        RefreshSpringVisual();
        RefreshCharacterVisual();
        RefreshShieldVisual();
        RefreshChargeEffectParent();
    }

    private void ApplyStats()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats is missing.");
            return;
        }

        playerStats.ApplyEquippedItems(equippedCharacter, equippedSpring);

        Debug.Log(
            $"Stats -> JumpForce: {playerStats.CurrentJumpForce}, " +
            $"ExtraAirJumps: {playerStats.ExtraAirJumps}, " +
            $"Shield: {playerStats.HasShield}, " +
            $"DoubleCoins: {playerStats.HasDoubleCoins}, " +
            $"ChargeEffect: {playerStats.HasChargeEffect}"
        );
    }

    private void RefreshSpringVisual()
    {
        if (springHolder == null)
            return;

        // Detach reusable visuals before clearing old spring
        if (chargeEffect != null)
            chargeEffect.transform.SetParent(null);

        if (shieldVisual != null)
            shieldVisual.transform.SetParent(null);

        // Destroy old spring visuals
        for (int i = springHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(springHolder.GetChild(i).gameObject);
        }

        currentSpringVisual = null;
        currentCharacterVisual = null;

        if (equippedSpring == null || equippedSpring.springVisualPrefab == null)
        {
            if (springJumpController != null)
                springJumpController.SetSpringVisual(null);

            return;
        }

        currentSpringVisual = Instantiate(equippedSpring.springVisualPrefab, springHolder);
        currentSpringVisual.transform.localPosition = Vector3.zero;
        currentSpringVisual.transform.localRotation = Quaternion.identity;
        currentSpringVisual.transform.localScale = Vector3.one;

        if (springJumpController != null)
            springJumpController.SetSpringVisual(currentSpringVisual);
    }
    private void RefreshCharacterVisual()
    {
        if (currentCharacterVisual != null)
            Destroy(currentCharacterVisual);

        if (equippedCharacter == null || equippedCharacter.characterVisualPrefab == null)
            return;

        if (currentSpringVisual == null)
            return;

        currentCharacterVisual = Instantiate(
            equippedCharacter.characterVisualPrefab,
            currentSpringVisual.transform
        );

        currentCharacterVisual.transform.localPosition = new Vector3(0f, characterYOffset, 0f);
        currentCharacterVisual.transform.localRotation = Quaternion.identity;
        currentCharacterVisual.transform.localScale = Vector3.one;
    }

    private void RefreshShieldVisual()
    {
        if (shieldVisual == null || playerStats == null)
            return;

        if (currentCharacterVisual != null)
        {
            shieldVisual.transform.SetParent(currentCharacterVisual.transform, false);
        }

        shieldVisual.SetActive(playerStats.HasShield);
    }

    private void RefreshChargeEffectParent()
    {
        if (chargeEffect == null || playerStats == null)
            return;

        if (currentSpringVisual != null)
        {
            chargeEffect.transform.SetParent(currentSpringVisual.transform, false);
            chargeEffect.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            chargeEffect.transform.localRotation = Quaternion.identity;
            chargeEffect.transform.localScale = Vector3.one;
        }

        chargeEffect.SetActive(false);

        if (springJumpController != null)
            springJumpController.SetAbsorbEffect(chargeEffect);

        Debug.Log("ChargeEffect parent is now: " +
                  (chargeEffect.transform.parent != null ? chargeEffect.transform.parent.name : "null"));
    }
    public void LoadFromPlayerData()
    {
        if (PlayerDataManager.Instance == null)
            return;

        equippedCharacter = PlayerDataManager.Instance.EquippedCharacter;
        equippedSpring = PlayerDataManager.Instance.EquippedSpring;

        ApplyAll();
    }

    public void EquipItemAndSave(ItemData item)
    {
        if (item == null)
            return;

        if (item.itemType == ItemType.Character)
        {
            equippedCharacter = item;
        }
        else if (item.itemType == ItemType.Spring)
        {
            equippedSpring = item;
        }

        ApplyAll();
    }
}