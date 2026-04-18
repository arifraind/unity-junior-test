using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopPopupUI shopPopupUI;
    [SerializeField] private SimpleEquipController equipController;
    [SerializeField] private PlayerStats playerStats;

    [Header("Data")]
    [SerializeField] private List<ShopItemData> characterItems = new List<ShopItemData>();
    [SerializeField] private List<ShopItemData> springItems = new List<ShopItemData>();

    [Tooltip("Filled automatically with bought/default items.")]
    [SerializeField] private List<ShopItemData> inventoryItems = new List<ShopItemData>();

    [Header("Prefab")]
    [SerializeField] private ShopItemHolder itemHolderPrefab;

    [Header("Content Parents")]
    [SerializeField] private Transform charactersContent;
    [SerializeField] private Transform springsContent;
    [SerializeField] private RectTransform inventoryContent;

    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    private readonly Dictionary<ShopItemData, bool> ownedStates = new Dictionary<ShopItemData, bool>();
    private readonly Dictionary<ShopItemData, bool> equippedStates = new Dictionary<ShopItemData, bool>();

    private readonly List<ShopItemHolder> spawnedHolders = new List<ShopItemHolder>();
    private readonly List<ShopItemHolder> inventoryHolders = new List<ShopItemHolder>();

    private void Start()
    {
        BuildShop();
        LoadStateFromPlayerData();
        RebuildInventoryUI();
        RefreshAllVisibleHolders();
    }
    private void LoadStateFromPlayerData()
    {
        ownedStates.Clear();
        equippedStates.Clear();
        inventoryItems.Clear();

        // Initialize all items as not owned / not equipped
        for (int i = 0; i < characterItems.Count; i++)
        {
            if (characterItems[i] != null)
            {
                ownedStates[characterItems[i]] = false;
                equippedStates[characterItems[i]] = false;
            }
        }

        for (int i = 0; i < springItems.Count; i++)
        {
            if (springItems[i] != null)
            {
                ownedStates[springItems[i]] = false;
                equippedStates[springItems[i]] = false;
            }
        }

        if (PlayerDataManager.Instance == null)
        {
            // First-time fallback defaults
            if (characterItems.Count > 0 && characterItems[0] != null)
            {
                ownedStates[characterItems[0]] = true;
                equippedStates[characterItems[0]] = true;
                inventoryItems.Add(characterItems[0]);
            }

            if (springItems.Count > 0 && springItems[0] != null)
            {
                ownedStates[springItems[0]] = true;
                equippedStates[springItems[0]] = true;
                inventoryItems.Add(springItems[0]);
            }

            ApplyAllEquippedItems();
            return;
        }

        // Restore owned items
        List<ShopItemData> savedOwned = PlayerDataManager.Instance.GetOwnedItems();
        for (int i = 0; i < savedOwned.Count; i++)
        {
            ShopItemData item = savedOwned[i];
            if (item == null)
                continue;

            ownedStates[item] = true;

            if (!inventoryItems.Contains(item))
                inventoryItems.Add(item);
        }

        // Guarantee defaults exist at least once
        if (characterItems.Count > 0 && !ownedStates[characterItems[0]])
        {
            ownedStates[characterItems[0]] = true;
            inventoryItems.Add(characterItems[0]);
            PlayerDataManager.Instance.AddOwnedItem(characterItems[0]);
        }

        if (springItems.Count > 0 && !ownedStates[springItems[0]])
        {
            ownedStates[springItems[0]] = true;
            inventoryItems.Add(springItems[0]);
            PlayerDataManager.Instance.AddOwnedItem(springItems[0]);
        }

        // Restore equipped
        ShopItemData equippedCharacter = PlayerDataManager.Instance.EquippedCharacter;
        ShopItemData equippedSpring = PlayerDataManager.Instance.EquippedSpring;

        if (equippedCharacter != null)
            equippedStates[equippedCharacter] = true;
        else if (characterItems.Count > 0)
            equippedStates[characterItems[0]] = true;

        if (equippedSpring != null)
            equippedStates[equippedSpring] = true;
        else if (springItems.Count > 0)
            equippedStates[springItems[0]] = true;

        SortInventoryItems();
        ApplyAllEquippedItems();

        if (equipController != null)
        {
            if (PlayerDataManager.Instance.EquippedCharacter != null)
                equipController.EquipItem(PlayerDataManager.Instance.EquippedCharacter);

            if (PlayerDataManager.Instance.EquippedSpring != null)
                equipController.EquipItem(PlayerDataManager.Instance.EquippedSpring);
        }
    }

    private void BuildShop()
    {
        SpawnCategory(characterItems, charactersContent);
        SpawnCategory(springItems, springsContent);
    }

    private void SpawnCategory(List<ShopItemData> items, Transform parent)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ShopItemData item = items[i];

            if (!ownedStates.ContainsKey(item))
                ownedStates[item] = false;

            if (!equippedStates.ContainsKey(item))
                equippedStates[item] = false;

            ShopItemHolder holder = Instantiate(itemHolderPrefab, parent, false);
            holder.Setup(item, this);
            spawnedHolders.Add(holder);
        }
    }

    private void SetDefaultOwnedAndEquipped()
    {
        // First character default
        if (characterItems.Count > 0 && characterItems[0] != null)
        {
            ShopItemData firstCharacter = characterItems[0];
            ownedStates[firstCharacter] = true;
            equippedStates[firstCharacter] = true;

            if (!inventoryItems.Contains(firstCharacter))
                inventoryItems.Add(firstCharacter);
        }

        // First spring default
        if (springItems.Count > 0 && springItems[0] != null)
        {
            ShopItemData firstSpring = springItems[0];
            ownedStates[firstSpring] = true;
            equippedStates[firstSpring] = true;

            if (!inventoryItems.Contains(firstSpring))
                inventoryItems.Add(firstSpring);
        }

        SortInventoryItems();
        ApplyAllEquippedItems();

        if (equipController != null)
        {
            if (characterItems.Count > 0 && characterItems[0] != null)
                equipController.EquipItem(characterItems[0]);

            if (springItems.Count > 0 && springItems[0] != null)
                equipController.EquipItem(springItems[0]);
        }
    }

    public bool IsOwned(ShopItemData item)
    {
        return ownedStates.ContainsKey(item) && ownedStates[item];
    }

    public bool IsEquipped(ShopItemData item)
    {
        return equippedStates.ContainsKey(item) && equippedStates[item];
    }

    public void OnItemButtonPressed(ShopItemData item)
    {
        if (!IsOwned(item))
        {
            TryBuyWithPopup(item);
        }
        else if (!IsEquipped(item))
        {
            Equip(item);
            RefreshAllVisibleHolders();
        }
    }

    private void CompletePurchase(ShopItemData item)
    {
        if (playerInventory == null || item == null)
            return;

        if (playerInventory.CurrentCoins < item.price)
        {
            Debug.Log("Purchase failed: not enough coins.");
            return;
        }

        playerInventory.AddCoins(-item.price);
        ownedStates[item] = true;

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.AddOwnedItem(item);

        AddToInventory(item);

        // Optional: auto-equip after buy
        Equip(item);

        RefreshAllVisibleHolders();

        Debug.Log("Bought: " + item.itemName);
    }

    private void AddToInventory(ShopItemData item)
    {
        if (item.itemType != ItemType.Character && item.itemType != ItemType.Spring)
            return;

        if (!inventoryItems.Contains(item))
        {
            inventoryItems.Add(item);
            SortInventoryItems();
            RebuildInventoryUI();
        }
    }

    private void SortInventoryItems()
    {
        inventoryItems.Sort((a, b) =>
        {
            int aOrder = GetInventoryOrder(a.itemType);
            int bOrder = GetInventoryOrder(b.itemType);

            if (aOrder != bOrder)
                return aOrder.CompareTo(bOrder);

            return a.itemName.CompareTo(b.itemName);
        });
    }

    private int GetInventoryOrder(ItemType type)
    {
        switch (type)
        {
            case ItemType.Character: return 0;
            case ItemType.Spring: return 1;
            default: return 2;
        }
    }

    private void RebuildInventoryUI()
    {
        if (inventoryContent == null)
            return;

        for (int i = 0; i < inventoryHolders.Count; i++)
        {
            if (inventoryHolders[i] != null)
                Destroy(inventoryHolders[i].gameObject);
        }

        inventoryHolders.Clear();

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            ShopItemData item = inventoryItems[i];
            ShopItemHolder holder = Instantiate(itemHolderPrefab, inventoryContent, false);
            holder.Setup(item, this);
            inventoryHolders.Add(holder);
        }

        ForceResizeInventoryContent();
    }

    private void ForceResizeInventoryContent()
    {
        int itemCount = inventoryItems.Count;

        float heightPerItem = 3f; // 🔥 your rule
        float newHeight = itemCount * heightPerItem;

        Vector2 size = inventoryContent.sizeDelta;
        size.y = newHeight;
        inventoryContent.sizeDelta = size;
    }

    private void Equip(ShopItemData item)
    {
        List<ShopItemData> targetList = GetListByType(item.itemType);

        for (int i = 0; i < targetList.Count; i++)
        {
            equippedStates[targetList[i]] = false;
        }

        equippedStates[item] = true;

        if (PlayerDataManager.Instance != null)
            PlayerDataManager.Instance.EquipItem(item);

        ApplyAllEquippedItems();

        if (equipController != null)
        {
            equipController.EquipItem(item);
        }

        Debug.Log("Equipped: " + item.itemName);
    }

    private void ApplyAllEquippedItems()
    {
        ShopItemData equippedCharacter = null;
        ShopItemData equippedSpring = null;

        foreach (var item in characterItems)
        {
            if (IsEquipped(item))
            {
                equippedCharacter = item;
                break;
            }
        }

        foreach (var item in springItems)
        {
            if (IsEquipped(item))
            {
                equippedSpring = item;
                break;
            }
        }

        if (playerStats != null)
            playerStats.ApplyEquippedItems(equippedCharacter, equippedSpring);
    }

    private List<ShopItemData> GetListByType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Character:
                return characterItems;
            case ItemType.Spring:
                return springItems;
            case ItemType.Item:
                return inventoryItems;
            default:
                return characterItems;
        }
    }

    private void RefreshAllVisibleHolders()
    {
        for (int i = 0; i < spawnedHolders.Count; i++)
        {
            if (spawnedHolders[i] != null)
                spawnedHolders[i].Refresh();
        }

        for (int i = 0; i < inventoryHolders.Count; i++)
        {
            if (inventoryHolders[i] != null)
                inventoryHolders[i].Refresh();
        }
    }
    private void TryBuyWithPopup(ShopItemData item)
    {
        if (playerInventory == null || item == null)
            return;

        int currentCoins = playerInventory.CurrentCoins;
        int price = item.price;

        if (currentCoins < price)
        {
            if (shopPopupUI != null)
            {
                shopPopupUI.ShowNotEnoughCoins(item.itemName, currentCoins, price);
            }
            else
            {
                Debug.Log($"Not enough coins. You have {currentCoins}/{price}");
            }

            return;
        }

        if (shopPopupUI != null)
        {
            shopPopupUI.ShowConfirmPurchase(item.itemName, price, () =>
            {
                CompletePurchase(item);
            });
        }
        else
        {
            CompletePurchase(item);
        }
    }
}