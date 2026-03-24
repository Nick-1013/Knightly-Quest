using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; // The speed at which the background moves relative to the camera
     void Awake()
    {
        startPos = transform.position.x; // Store the initial x position of the background
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x; // Store the initial x position of the background
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance the camera has moved from the starting position
        float distance = cam.transform.position.x * parallaxEffect; // 0 = no movement, 1 = same movement as the camera, 0.5 = half the movement of the camera

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z); // Move the background based on the calculated distance
    }
}
