using UnityEngine;
using System.Collections;

public class SkullRoller : MonoBehaviour
{
    public float rollSpeed = 10f;
    public float torqueAmount = -10f; // Negative for clockwise, positive for counter-clockwise
    public float timeUntilDestroy = 5f; // Time in seconds before the skull is destroyed
    private float direction = -1f; // Default to left, can be set to 1f for right
    private Rigidbody2D rb;

    public void SetDirection(float dir)
    {
        direction = Mathf.Sign(dir); // Normalize direction
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. Move forward immediately on spawn
        // Assuming forward is the X-axis (transform.right)
        rb.linearVelocity = new Vector2(direction, 0f) * rollSpeed;

        transform.localScale = new Vector3(direction, 1, 1);

        // 2. Apply spin to make it roll
        rb.AddTorque(torqueAmount * -direction, ForceMode2D.Force);

        StartCoroutine(DestroySkull());
    }

    IEnumerator DestroySkull()
    {
        yield return new WaitForSeconds(timeUntilDestroy);
        Destroy(gameObject);
    }
}
