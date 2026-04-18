using UnityEngine;

public class ShopUIButtons : MonoBehaviour
{
    public void BackToMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.BackToMainMenu();
    }
    public void Play()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Play();
    }
    public void Shop()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoToShop();
    }
}