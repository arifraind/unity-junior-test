using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public string name;
        public Image image;
        public Sprite pressedSprite;
        public Sprite unpressedSprite;
    }

    [Header("Tabs")]
    [SerializeField] private Tab characterTab;
    [SerializeField] private Tab springTab;
    [SerializeField] private Tab inventoryTab;

    [Header("Panels")]
    [SerializeField] private GameObject charactersPanel;
    [SerializeField] private GameObject springsPanel;
    [SerializeField] private GameObject inventoryPanel;

    private Tab currentTab;

    private void Start()
    {
        SelectTab(characterTab); // default
    }

    public void OnCharacterTabPressed()
    {
        SelectTab(characterTab);
    }

    public void OnSpringTabPressed()
    {
        SelectTab(springTab);
    }

    public void OnInventoryTabPressed()
    {
        SelectTab(inventoryTab);
    }

    private void SelectTab(Tab tab)
    {
        currentTab = tab;

        UpdateTab(characterTab);
        UpdateTab(springTab);
        UpdateTab(inventoryTab);

        UpdatePanels();
    }

    private void UpdateTab(Tab tab)
    {
        if (tab.image == null)
            return;

        if (tab == currentTab)
            tab.image.sprite = tab.pressedSprite;
        else
            tab.image.sprite = tab.unpressedSprite;
    }

    private void UpdatePanels()
    {
        if (currentTab == characterTab)
        {
            charactersPanel.SetActive(true);
            springsPanel.SetActive(false);
            inventoryPanel.SetActive(false);
        }
        else if (currentTab == springTab)
        {
            charactersPanel.SetActive(false);
            springsPanel.SetActive(true);
            inventoryPanel.SetActive(false);
        }
        else if (currentTab == inventoryTab)
        {
            charactersPanel.SetActive(false);
            springsPanel.SetActive(false);
            inventoryPanel.SetActive(true);
        }
    }
}