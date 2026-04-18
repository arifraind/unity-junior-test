using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class GameCoin : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int coinValue = 1;

    [Header("Spin Animation")]
    [SerializeField] private Sprite[] spinFrames;
    [SerializeField] private float framesPerSecond = 12f;

    [Header("Pickup")]
    [SerializeField] private string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private float animationTimer;
    private int currentFrame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        AnimateSpin();
    }

    private void AnimateSpin()
    {
        if (spinFrames == null || spinFrames.Length == 0)
            return;

        animationTimer += Time.deltaTime;
        float frameDuration = 1f / framesPerSecond;

        if (animationTimer >= frameDuration)
        {
            animationTimer -= frameDuration;
            currentFrame = (currentFrame + 1) % spinFrames.Length;
            spriteRenderer.sprite = spinFrames[currentFrame];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
            inventory.AddCoins(coinValue);

        Destroy(gameObject);
    }
}