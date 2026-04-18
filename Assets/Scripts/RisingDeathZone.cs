using UnityEngine;

public class RisingDeathZone : MonoBehaviour
{
    [Header("Rise Settings")]
    [SerializeField] private float startSpeed = 0.2f;
    [SerializeField] private float accelerationPerSecond = 0.02f;
    [SerializeField] private float maxSpeed = 2f;

    private float currentSpeed;

    private void Start()
    {
        currentSpeed = startSpeed;
    }

    private void Update()
    {
        currentSpeed += accelerationPerSecond * Time.deltaTime;
        currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        transform.position += Vector3.up * currentSpeed * Time.deltaTime;
    }
}