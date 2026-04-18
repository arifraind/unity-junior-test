using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private const string CoinsKey = "Player_Coins";
    private const string EquippedCharacterKey = "Equipped_Character";
    private const string EquippedSpringKey = "Equipped_Spring";
    private const string OwnedItemsKey = "Owned_Items";

    [Header("Default Values")]
    [SerializeField] private int startingCoins = 500;

    [Header("Coins")]
    [SerializeField] private int currentCoins = 0;

    [Header("Equipped")]
    [SerializeField] private ShopItemData equippedCharacter;
    [SerializeField] private ShopItemData equippedSpring;

    [Header("Owned Items")]
    [SerializeField] private List<ShopItemData> ownedItems = new List<ShopItemData>();

    [Header("Item Database")]
    [Tooltip("Put here every shop item that can be owned/equipped so saved data can be restored.")]
    [SerializeField] private List<ShopItemData> allShopItems = new List<ShopItemData>();

    public int CurrentCoins => currentCoins;
    public ShopItemData EquippedCharacter => equippedCharacter;
    public ShopItemData EquippedSpring => equippedSpring;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadData();
    }

    public void AddCoins(int amount)
    {
        currentCoins += amount;

        if (currentCoins < 0)
            currentCoins = 0;

        SaveData();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins < amount)
            return false;

        currentCoins -= amount;
        SaveData();
        return true;
    }

    public bool IsOwned(ShopItemData item)
    {
        return item != null && ownedItems.Contains(item);
    }

    public void AddOwnedItem(ShopItemData item)
    {
        if (item == null)
            return;

        if (!ownedItems.Contains(item))
        {
            ownedItems.Add(item);
            SaveData();
        }
    }

    public List<ShopItemData> GetOwnedItems()
    {
        return ownedItems;
    }

    public void EquipItem(ShopItemData item)
    {
        if (item == null)
            return;

        if (item.itemType == ItemType.Character)
            equippedCharacter = item;
        else if (item.itemType == ItemType.Spring)
            equippedSpring = item;

        AddOwnedItem(item);
        SaveData();
    }

    public void EquipCharacter(ShopItemData item)
    {
        if (item == null)
            return;

        equippedCharacter = item;
        AddOwnedItem(item);
        SaveData();
    }

    public void EquipSpring(ShopItemData item)
    {
        if (item == null)
            return;

        equippedSpring = item;
        AddOwnedItem(item);
        SaveData();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(CoinsKey, currentCoins);

        PlayerPrefs.SetString(EquippedCharacterKey, equippedCharacter != null ? equippedCharacter.name : string.Empty);
        PlayerPrefs.SetString(EquippedSpringKey, equippedSpring != null ? equippedSpring.name : string.Empty);

        string ownedItemsString = BuildOwnedItemsString();
        PlayerPrefs.SetString(OwnedItemsKey, ownedItemsString);

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentCoins = PlayerPrefs.GetInt(CoinsKey, startingCoins);

        ownedItems.Clear();

        string savedOwnedItems = PlayerPrefs.GetString(OwnedItemsKey, string.Empty);
        if (!string.IsNullOrEmpty(savedOwnedItems))
        {
            string[] ownedNames = savedOwnedItems.Split('|');
            for (int i = 0; i < ownedNames.Length; i++)
            {
                ShopItemData item = FindItemByName(ownedNames[i]);
                if (item != null && !ownedItems.Contains(item))
                    ownedItems.Add(item);
            }
        }

        string equippedCharacterName = PlayerPrefs.GetString(EquippedCharacterKey, string.Empty);
        if (!string.IsNullOrEmpty(equippedCharacterName))
            equippedCharacter = FindItemByName(equippedCharacterName);

        string equippedSpringName = PlayerPrefs.GetString(EquippedSpringKey, string.Empty);
        if (!string.IsNullOrEmpty(equippedSpringName))
            equippedSpring = FindItemByName(equippedSpringName);
    }

    private string BuildOwnedItemsString()
    {
        if (ownedItems == null || ownedItems.Count == 0)
            return string.Empty;

        List<string> itemNames = new List<string>();

        for (int i = 0; i < ownedItems.Count; i++)
        {
            if (ownedItems[i] != null)
                itemNames.Add(ownedItems[i].name);
        }

        return string.Join("|", itemNames);
    }

    private ShopItemData FindItemByName(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
            return null;

        for (int i = 0; i < allShopItems.Count; i++)
        {
            if (allShopItems[i] != null && allShopItems[i].name == itemName)
                return allShopItems[i];
        }

        return null;
    }

    [ContextMenu("Reset Saved Data")]
    public void ResetSavedData()
    {
        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.DeleteKey(EquippedCharacterKey);
        PlayerPrefs.DeleteKey(EquippedSpringKey);
        PlayerPrefs.DeleteKey(OwnedItemsKey);
        PlayerPrefs.Save();

        currentCoins = startingCoins;
        equippedCharacter = null;
        equippedSpring = null;
        ownedItems.Clear();
    }
}