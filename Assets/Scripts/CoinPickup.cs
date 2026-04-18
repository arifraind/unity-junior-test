using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;

    [Header("Spin Animation")]
    [SerializeField] private Sprite[] spinFrames;
    [SerializeField] private float framesPerSecond = 12f;

    [Header("Pickup")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool destroyOnPickup = true;

    [Header("Ground")]
    [SerializeField] private string groundTag = "Ground";

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private float animationTimer;
    private int currentFrame;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
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
        {
            inventory.AddCoins(coinValue);
        }

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(groundTag))
            return;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}