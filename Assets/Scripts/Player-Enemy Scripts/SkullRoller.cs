using UnityEngine;

public class SkullRoller : MonoBehaviour
{
    public float rollSpeed = 10f;
    public float torqueAmount = -10f; // Negative for clockwise, positive for counter-clockwise
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 1. Move forward immediately on spawn
        // Assuming forward is the X-axis (transform.right)
        rb.linearVelocity = new Vector2(1f, 0f) * rollSpeed;

        // 2. Apply spin to make it roll
        rb.AddTorque(torqueAmount, ForceMode2D.Force);
    }
}
