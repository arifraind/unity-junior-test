using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPopupUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private RectTransform cancelButtonRect;
    [SerializeField] private RectTransform confirmButtonRect;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    [Header("Layout")]
    [SerializeField] private Vector2 cancelCenteredPosition = new Vector2(0f, -29f);
    [SerializeField] private Vector2 cancelLeftPosition = new Vector2(-95f, -29f);
    [SerializeField] private Vector2 confirmRightPosition = new Vector2(95f, -29f);

    private Action confirmAction;

    private void Awake()
    {
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();

        cancelButton.onClick.AddListener(OnCancelPressed);
        confirmButton.onClick.AddListener(OnConfirmPressed);

        Hide();
    }

    public void ShowNotEnoughCoins(string itemName, int currentCoins, int requiredCoins)
    {
        if (root != null)
            root.SetActive(true);

        int missing = Mathf.Max(0, requiredCoins - currentCoins);

        messageText.text =

            $"You currently have\n" +
            $"<b>{currentCoins}</b> coins.\n" +
            $"You're missing\n" +
            $"<b>{missing}</b> more.";

        cancelButtonRect.anchoredPosition =
            new Vector2(0f, cancelButtonRect.anchoredPosition.y); confirmButton.gameObject.SetActive(false);

        confirmAction = null;
    }

    public void ShowConfirmPurchase(string itemName, int price, Action onConfirm)
    {
        if (root != null)
            root.SetActive(true);

        messageText.text =
            $"Are you sure\nyou want to buy\n\n<b>{itemName}</b>\n\nfor <b>{price}</b> coins?";

        // ✅ RESET layout back to original
        cancelButtonRect.anchoredPosition = cancelLeftPosition;
        confirmButtonRect.anchoredPosition = confirmRightPosition;

        confirmButton.gameObject.SetActive(true);

        confirmAction = onConfirm;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);

        confirmAction = null;
    }

    private void OnCancelPressed()
    {
        Hide();
    }

    private void OnConfirmPressed()
    {
        confirmAction?.Invoke();
        Hide();
    }
}