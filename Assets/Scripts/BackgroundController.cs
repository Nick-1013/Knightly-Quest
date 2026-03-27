using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public enum ParallaxMode
    {
        Static,
        FollowCamera,
        Looping
    }

    public ParallaxMode mode = ParallaxMode.FollowCamera;

    public GameObject cam;
    public float parallaxEffect = 0.5f;

    // Replace single background with an array so multiple layers are supported
    public GameObject[] backgrounds;

    // store each background's original x position
    private float[] startPositions;

    // example of a List that can be manipulated at runtime
    private readonly List<GameObject> activeBackgrounds = new();

    void Awake()
    {
        // Defensive initialization
        if (backgrounds == null)
            backgrounds = new GameObject[0];

        startPositions = new float[backgrounds.Length];

        // Populate activeBackgrounds using a while loop
        int idx = 0;
        while (idx < backgrounds.Length)
        {
            if (backgrounds[idx] != null)
            {
                activeBackgrounds.Add(backgrounds[idx]);
            }
            idx++;
        }
    }

    void Start()
    {
        // Initialize startPositions using a for loop over the array
        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null)
                startPositions[i] = backgrounds[i].transform.position.x;
        }

        // Example foreach loop over the List to log names (can be removed in production)
        foreach (var bg in activeBackgrounds)
        {
            if (bg != null)
                Debug.Log($"Active background: {bg.name}");
        }
    }

    void Update()
    {
        if (cam == null || backgrounds == null)
            return;

        float distance = cam.transform.position.x * parallaxEffect;

        // 1st for loop: update each background from the array by index
        for (int i = 0; i < backgrounds.Length; i++)
        {
            var bg = backgrounds[i];
            if (bg == null) continue;

            Vector3 pos = bg.transform.position;
            pos.x = startPositions[i] + distance;
            bg.transform.position = pos;
        }

        // 2nd foreach loop: apply additional behaviour to the active list (example: simple visibility toggle)
        foreach (var bg in activeBackgrounds)
        {
            if (bg == null) continue;
            // Example: if in Looping mode, keep x within a range (simple placeholder logic)
            if (mode == ParallaxMode.Looping)
            {
                var p = bg.transform.position;
                if (p.x > startPositions[0] + 50f) // arbitrary range example
                    p.x = startPositions[0] - 50f;
                bg.transform.position = p;
            }
        }

        // 3rd for-each style loop: iterate child transforms (demonstrates another foreach)
        foreach (Transform child in transform)
        {
            // lightweight operation on children, e.g., slight vertical bob for effect
            var cp = child.localPosition;
            cp.y += Mathf.Sin(Time.time) * 0.0001f;
            child.localPosition = cp;
        }
    }
}
