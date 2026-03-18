using UnityEngine;
using UnityEngine.UI; // Optional: needed if you want to use the Unity UI components (like a Slider or Image fill) for a health bar.

public class Health : MonoBehaviour
{
    // These fields are public/SerializeField so they can be set in the Unity Inspector
    public float maxHealth = 100f;
    private Enemy enemy; // Reference to the Enemy script to call EnemyKilled() when this entity dies
    private PlayerMovement player; // Reference to the PlayerMovement script to call GameOver() when the player dies
    [SerializeField] private float currentHealth;
    [SerializeField] private ImageController healthUI;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public float invulnerabilityTime = 0.5f;
    private float invulTimer;

    // Reference to the UI element (e.g., a Slider or Image) to visually represent health
    // Make sure to add 'using UnityEngine.UI;' at the top of your script for this.
    // [SerializeField] private Slider healthSlider; // Use if you have a Slider
    // [SerializeField] private Image healthBarFill; // Use if you have an Image with type set to 'Filled'

    void Start()
    {
        // When the game starts, set the current health to the maximum health
        currentHealth = maxHealth;
        // UpdateHealthUI(); // Call this to set the initial state of the UI
        enemy = GetComponent<Enemy>();
        player = GetComponent<PlayerMovement>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (invulTimer > 0)
        {
            invulTimer -= Time.deltaTime;

            FlashEffect();
        }
        else
        {
            ResetVisual();
        }
    }

    void FlashEffect()
    {
        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;

        // PingPong creates a smooth fade in/out
        float alpha = Mathf.PingPong(Time.time * 5f, 1f);

        color.a = alpha;
        spriteRenderer.color = color;
    }

    void ResetVisual()
    {
        if (spriteRenderer == null) return;

        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    // Public function to allow other scripts to deal damage
    public void TakeDamage(float amount)
    {
        if (invulTimer > 0) return;

        currentHealth -= amount;
        // Use Mathf.Clamp to ensure health stays between 0 and maxHealth
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        invulTimer = invulnerabilityTime;

        if (healthUI != null)
            healthUI.TakeDamage((int)amount);

        // UpdateHealthUI(); // Update the UI when health changes

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Public function to allow other scripts to heal the entity
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // UpdateHealthUI(); // Update the UI when health changes

        if (healthUI != null)
            healthUI.Heal((int)amount);
    }

    // Optional: Function to update the health bar UI
    /*
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }
    */

    private void Die()
    {
        // Handle death logic here (e.g., play death animation, respawn, destroy object, reload scene)
        Debug.Log(gameObject.name + " has died!");
        // For example, disable the game object:
        // gameObject.SetActive(false);

        Enemy enemy = GetComponent<Enemy>();
        PlayerMovement player = GetComponent<PlayerMovement>();

        if (enemy != null)
        {
            enemy.Die();
        }

        if (player != null)
        {
            GameManagerScript gm = FindFirstObjectByType<GameManagerScript>();

            if (gm != null)
                gm.GameOver();
        }
    }
}
