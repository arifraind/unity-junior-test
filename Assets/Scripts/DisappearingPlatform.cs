using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float disappearDelay = 0.15f;
    [SerializeField] private float disableTime = 1.5f;
    [SerializeField] private Collider2D platformCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool triggered;

    private void Reset()
    {
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (triggered) return;

        if (!collision.collider.CompareTag("Player"))
            return;

        // Only trigger when player lands from above
        if (collision.relativeVelocity.y <= 0f)
        {
            triggered = true;
            StartCoroutine(DisappearRoutine());
        }
    }

    private IEnumerator DisappearRoutine()
    {
        yield return new WaitForSeconds(disappearDelay);

        if (platformCollider != null)
            platformCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(disableTime);

        Destroy(gameObject);
    }
}