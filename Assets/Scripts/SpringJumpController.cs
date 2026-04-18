using UnityEngine;
using UnityEngine.InputSystem;

public class SpringJumpController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject springVisual;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Spring Scale")]
    [SerializeField] private float releasedYScale = 0.25f;
    [SerializeField] private float minSqueezeYScale = 0.18f;
    [SerializeField] private float maxSqueezeYScale = 0.10f;

    [Header("Drag Settings")]
    [SerializeField] private float maxDownDragPixels = 300f;
    [SerializeField] private float maxSideDragPixels = 250f;

    [Header("Rotation")]
    [SerializeField] private float maxRotationZ = 30f;

    [Header("Jump Force")]
    [SerializeField] private float minUpForceMultiplier = 0.2f;
    [SerializeField] private float maxSideForce = 6f;

    [Header("Charge Effect")]
    [SerializeField] private GameObject absorbEffect;
    [SerializeField] private float absorbEffectThreshold = 0.95f;
    [SerializeField] private float minForceBonusForEffect = 1.1f;

    private Vector3 springStartScale;
    private Vector3 springStartEuler;
    private Vector2 pressStartScreenPos;
    private Vector2 currentScreenPos;

    private bool isPressing;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isDead;

    private int jumpsRemaining;

    public int JumpsRemaining => jumpsRemaining;
    public int TotalJumpsAvailable => 1 + playerStats.ExtraAirJumps;
    public bool IsGrounded => isGrounded;
    public float CurrentChargePercent { get; private set; }
    public bool IsPressing => isPressing;
    public bool IsFullyCharged => CurrentChargePercent >= absorbEffectThreshold;
    public bool IsDead => isDead;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (springVisual != null)
        {
            springStartScale = springVisual.transform.localScale;
            springStartEuler = springVisual.transform.localEulerAngles;
        }

        if (absorbEffect != null)
            absorbEffect.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        ResetSpringVisual();
        ResetJumpCount();
    }

    private void Update()
    {
        if (isDead)
            return;

        CheckGroundedState();
        HandlePointerInput();

        if (isPressing)
            UpdateSpringVisual();

        UpdateChargeEffect();
    }

    private void CheckGroundedState()
    {
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
            ResetJumpCount();
    }

    private void ResetJumpCount()
    {
        jumpsRemaining = 1 + playerStats.ExtraAirJumps;
    }

    private void HandlePointerInput()
    {
        if (Touchscreen.current != null)
        {
            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.wasPressedThisFrame)
                StartPress(primaryTouch.position.ReadValue());

            if (primaryTouch.press.isPressed)
                currentScreenPos = primaryTouch.position.ReadValue();

            if (primaryTouch.press.wasReleasedThisFrame)
            {
                currentScreenPos = primaryTouch.position.ReadValue();
                ReleaseJump();
            }

            return;
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartPress(Mouse.current.position.ReadValue());

            if (Mouse.current.leftButton.isPressed)
                currentScreenPos = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                currentScreenPos = Mouse.current.position.ReadValue();
                ReleaseJump();
            }
        }
    }

    private void StartPress(Vector2 screenPos)
    {
        if (isDead)
            return;

        if (jumpsRemaining <= 0)
            return;

        isPressing = true;
        pressStartScreenPos = screenPos;
        currentScreenPos = screenPos;
        CurrentChargePercent = 0f;
    }

    private void UpdateSpringVisual()
    {
        if (springVisual == null)
            return;

        Vector2 drag = currentScreenPos - pressStartScreenPos;

        float downDrag = Mathf.Max(0f, -drag.y);
        float downT = Mathf.Clamp01(downDrag / maxDownDragPixels);
        CurrentChargePercent = downT;

        float sideT = Mathf.Clamp(drag.x / maxSideDragPixels, -1f, 1f);

        float targetY = Mathf.Lerp(minSqueezeYScale, maxSqueezeYScale, downT);
        float targetRotationZ = sideT * maxRotationZ;

        Vector3 scale = springStartScale;
        scale.y = targetY;
        springVisual.transform.localScale = scale;

        springVisual.transform.localRotation = Quaternion.Euler(
            springStartEuler.x,
            springStartEuler.y,
            springStartEuler.z + targetRotationZ
        );
    }

    private void UpdateChargeEffect()
    {
        if (absorbEffect == null || playerStats == null)
            return;

        bool hasBonus = playerStats.HasSignificantForceBonus(minForceBonusForEffect);
        bool hasChargeEffect = playerStats.HasChargeEffect;

        bool shouldShow =
            isPressing &&
            CurrentChargePercent >= absorbEffectThreshold &&
            hasBonus &&
            hasChargeEffect;

        absorbEffect.SetActive(shouldShow);
    }

    private void ReleaseJump()
    {
        if (isDead)
            return;

        if (!isPressing)
            return;

        if (jumpsRemaining <= 0)
        {
            isPressing = false;
            ResetSpringVisual();
            return;
        }

        isPressing = false;

        Vector2 drag = currentScreenPos - pressStartScreenPos;

        float downDrag = Mathf.Max(0f, -drag.y);
        float downT = Mathf.Clamp01(downDrag / maxDownDragPixels);

        float sideT = Mathf.Clamp(drag.x / maxSideDragPixels, -1f, 1f);

        float maxForce = playerStats.CurrentJumpForce;
        float upForce = Mathf.Lerp(maxForce * minUpForceMultiplier, maxForce, downT);
        float sideForce = -sideT * maxSideForce;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.AddForce(new Vector2(sideForce, upForce), ForceMode2D.Impulse);

        jumpsRemaining--;
        ResetSpringVisual();
    }

    private void ResetSpringVisual()
    {
        if (springVisual == null)
            return;

        Vector3 scale = springStartScale;
        scale.y = releasedYScale;
        springVisual.transform.localScale = scale;

        springVisual.transform.localRotation = Quaternion.Euler(springStartEuler);
        CurrentChargePercent = 0f;

        if (absorbEffect != null)
            absorbEffect.SetActive(false);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isPressing = false;

        ResetSpringVisual();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void SetSpringVisual(GameObject newSpringVisual)
    {
        springVisual = newSpringVisual;

        if (springVisual != null)
        {
            springStartScale = springVisual.transform.localScale;
            springStartEuler = springVisual.transform.localEulerAngles;
            ResetSpringVisual();
        }
    }

    public void SetAbsorbEffect(GameObject newAbsorbEffect)
    {
        absorbEffect = newAbsorbEffect;

        if (absorbEffect != null)
        {
            // Keep current world scale
            Vector3 originalWorldScale = absorbEffect.transform.lossyScale;

            // Optional: if you want it to follow the spring/player
            absorbEffect.transform.SetParent(transform, true);

            // Restore scale so parenting does not squash/stretch it
            Vector3 parentScale = absorbEffect.transform.parent != null
                ? absorbEffect.transform.parent.lossyScale
                : Vector3.one;

            absorbEffect.transform.localScale = new Vector3(
                originalWorldScale.x / parentScale.x,
                originalWorldScale.y / parentScale.y,
                originalWorldScale.z / parentScale.z
            );

            absorbEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
            return;

        if (other.CompareTag("Death"))
            Die();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}