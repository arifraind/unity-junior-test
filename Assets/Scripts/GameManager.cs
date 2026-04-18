using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private string shopScene = "Shop";

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 🔙 Back to Main Menu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    // ▶️ Start Game
    public void Play()
    {
        SceneManager.LoadScene(gameScene);
    }

    // 🛒 Go to Shop
    public void GoToShop()
    {
        SceneManager.LoadScene(shopScene);
    }
}