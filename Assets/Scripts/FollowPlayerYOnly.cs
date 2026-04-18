using UnityEngine;

public class FollowPlayerYOnly : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset = 0f;
    [SerializeField] private bool smoothFollow = false;
    [SerializeField] private float followSpeed = 10f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = transform.position;
        float targetY = target.position.y + yOffset;

        if (smoothFollow)
            pos.y = Mathf.Lerp(pos.y, targetY, followSpeed * Time.deltaTime);
        else
            pos.y = targetY;

        transform.position = pos;
    }
}