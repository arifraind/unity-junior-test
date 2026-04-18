using UnityEngine;

public class ShieldSpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer shieldRenderer;

    [Header("Animation")]
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

    private int currentFrame;
    private float timer;
    private bool isPlaying;

    private void Awake()
    {
        if (shieldRenderer == null)
            shieldRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (animationFrames != null && animationFrames.Length > 0)
        {
            shieldRenderer.sprite = animationFrames[0];
        }

        if (playOnStart)
            Play();
        else
            Stop();
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        if (animationFrames == null || animationFrames.Length == 0 || shieldRenderer == null)
            return;

        timer += Time.deltaTime;

        float frameTime = 1f / frameRate;

        if (timer >= frameTime)
        {
            timer -= frameTime;
            currentFrame++;

            if (currentFrame >= animationFrames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = animationFrames.Length - 1;
                    isPlaying = false;
                }
            }

            shieldRenderer.sprite = animationFrames[currentFrame];
        }
    }

    public void Play()
    {
        if (animationFrames == null || animationFrames.Length == 0)
            return;

        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
        timer = 0f;
        currentFrame = 0;

        if (animationFrames != null && animationFrames.Length > 0 && shieldRenderer != null)
            shieldRenderer.sprite = animationFrames[0];
    }

    public void RestartAnimation()
    {
        timer = 0f;
        currentFrame = 0;

        if (animationFrames != null && animationFrames.Length > 0 && shieldRenderer != null)
            shieldRenderer.sprite = animationFrames[0];

        isPlaying = true;
    }

    public void ShowShield(bool show)
    {
        gameObject.SetActive(show);
    }
}