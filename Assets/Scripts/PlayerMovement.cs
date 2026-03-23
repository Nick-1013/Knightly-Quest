using UnityEngine; // Gives access to Unity engine core features (physics, transforms, etc.)
using UnityEngine.InputSystem; // Enables use of the new Unity Input System
using System.Linq; // Allows use of LINQ (used here to find gamepad devices)

public class PlayerMovement : MonoBehaviour // Main player controller class
{
    // ---------------- GAME MANAGER ----------------
    private GameManagerScript gameManager; // Reference to GameManager for pause/game over logic

    // ---------------- MOVEMENT SETTINGS ----------------
    public float speed = 5.0f; // Base movement speed of the player
    public float runMultiplier = 1.75f; // Multiplier applied when running
    public float jumpForce = 10.0f; // Force applied when jumping
    public int maxJumps = 2; // Maximum number of jumps allowed (double jump)
    public float groundPoundForce = 25f; // Downward force for ground pound

    // ---------------- MOVEMENT STATE ----------------
    private float moveDirection; // Stores horizontal input (-1 to 1)
    private Rigidbody2D rb; // Reference to Rigidbody2D for physics movement
    private Animator animator; // Reference to Animator for animations
    private bool isGrounded; // Tracks if player is touching the ground
    private int availableJumps; // Tracks how many jumps remain
    private bool isGroundPounding; // Tracks if player is currently ground pounding

    // ---------------- ATTACK SETTINGS ----------------
    [Header("Attack")] // Creates a header in the Unity Inspector
    public float attackRange = 2.5f; // Radius of attack hit detection
    public int attackDamage = 1; // Damage dealt per attack
    public Transform attackPoint;

    public LayerMask enemyLayer; // Layer mask to specify which layers are considered enemies for attack detection

    // ---------------- UNITY START ----------------
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D component
        animator = GetComponent<Animator>(); // Get Animator component
        availableJumps = maxJumps; // Initialize jump count

        gameManager = FindFirstObjectByType<GameManagerScript>(); // Find GameManager in scene
    }

    // ---------------- MAIN UPDATE LOOP ----------------
    void Update()
    {
        if (Time.timeScale == 0f) return; // Stop input when game is paused

        // Get current connected gamepad (if any)
        Gamepad currentGamepad = InputSystem.devices.OfType<Gamepad>().FirstOrDefault();

        // Get horizontal movement input (keyboard or gamepad)
        moveDirection = GetHorizontalInput(currentGamepad);

        // Apply deadzone to prevent small unwanted movement
        if (Mathf.Abs(moveDirection) < 0.1f) moveDirection = 0f;

        float currentSpeed = speed; // Start with base speed

        // Apply run multiplier if run input is held
        if (IsRunHeld(currentGamepad))
        {
            currentSpeed *= runMultiplier;
        }

        // Only allow movement if NOT ground pounding
        if (!isGroundPounding)
        {
            // Apply horizontal velocity while keeping vertical velocity unchanged
            rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);

            // Flip character based on movement direction
            if (moveDirection > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveDirection < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        // Update animation states
        UpdateAnimations();

        // -------- GROUND POUND CHECK --------
        // Only allow ground pound while in air and not already doing it
        if (!isGrounded && !isGroundPounding && IsGroundPoundPressed(currentGamepad))
        {
            GroundPound(); // Execute ground pound
            return; // Skip rest of update this frame
        }

        // -------- JUMP CHECK --------
        // Only jump if player has jumps remaining
        if (IsJumpPressed(currentGamepad) && availableJumps > 0)
        {
            Jump(); // Perform jump
        }

        // -------- ATTACK INPUT --------
        // Check for mouse click (left click)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack(); // Trigger attack
        }
    }

    // ---------------- ANIMATION HANDLER ----------------
    void UpdateAnimations()
    {
        if (animator == null) return; // Safety check

        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x); // Get horizontal movement speed
        float verticalSpeed = rb.linearVelocity.y; // Get vertical velocity

        bool isMoving = horizontalSpeed > 0.1f; // True if moving horizontally
        bool isJumping = verticalSpeed > 0.1f && !isGrounded; // True if rising
        bool isFalling = verticalSpeed < -0.1f && !isGrounded; // True if falling

        // Apply animation parameters
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);
    }

    // ---------------- INPUT HANDLING ----------------
    private float GetHorizontalInput(Gamepad currentGamepad)
    {
        float gamepadInput = 0f; // Store gamepad input
        float keyboardInput = 0f; // Store keyboard input

        // -------- GAMEPAD INPUT --------
        if (currentGamepad != null)
        {
            gamepadInput = currentGamepad.leftStick.x.ReadValue(); // Read left stick horizontal value
        }

        // -------- KEYBOARD INPUT --------
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                keyboardInput -= 1f; // Move left

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                keyboardInput += 1f; // Move right
        }

        // Prefer gamepad input if it's being used
        if (Mathf.Abs(gamepadInput) > 0.1f)
            return gamepadInput;

        return keyboardInput; // Otherwise use keyboard
    }

    // ---------------- JUMP LOGIC ----------------
    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply upward force

        isGrounded = false; // Immediately mark as airborne
        availableJumps--; // Reduce available jumps
    }

    // ---------------- GROUND POUND ----------------
    private void GroundPound()
    {
        isGroundPounding = true; // Mark as ground pounding

        rb.linearVelocity = Vector2.zero; // Stop all current motion
        rb.AddForce(Vector2.down * groundPoundForce, ForceMode2D.Impulse); // Slam downward
    }

    // ---------------- INPUT CHECKS ----------------
    private bool IsJumpPressed(Gamepad currentGamepad)
    {
        bool gamepadJump = currentGamepad != null && currentGamepad.aButton.wasPressedThisFrame; // Gamepad A button
        bool keyboardJump = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame; // Space key

        return gamepadJump || keyboardJump; // Return true if either pressed
    }

    private bool IsGroundPoundPressed(Gamepad currentGamepad)
    {
        bool gamepadPound =
            currentGamepad != null &&
            currentGamepad.leftStick.y.ReadValue() < -0.5f && // Stick pushed down
            currentGamepad.aButton.wasPressedThisFrame; // Jump pressed

        bool keyboardPound =
            Keyboard.current != null &&
            (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && // Down input
            Keyboard.current.spaceKey.wasPressedThisFrame; // Jump pressed

        return gamepadPound || keyboardPound; // Return true if either triggered
    }

    private bool IsRunHeld(Gamepad currentGamepad)
    {
        bool gamepadRun =
            currentGamepad != null &&
            (currentGamepad.leftStickButton.isPressed || currentGamepad.rightTrigger.ReadValue() > 0.1f); // Run input

        bool keyboardRun =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed; // Shift key

        return gamepadRun || keyboardRun; // Return true if either held
    }

    // ---------------- COLLISION HANDLING ----------------
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Check if collided with ground
        {
            if (isGroundPounding)
            {
                rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse); // Small bounce after ground pound
            }

            availableJumps = maxJumps; // Reset jumps
            isGrounded = true; // Mark as grounded
            isGroundPounding = false; // Reset ground pound state
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Mark as airborne
        }
    }

    // ---------------- ATTACK SYSTEM ----------------
    void Attack() // FIXED: removed unnecessary parameter
    {
        Debug.Log("[Player] ATTACK!"); // Debug log for attack trigger

        if (animator != null)
            animator.SetTrigger("IsAttacking"); // Trigger attack animation

        // Detect all colliders within attack range
        Vector2 attackCenter = attackPoint.position;

        // Draw this always
        Debug.DrawLine(transform.position, attackCenter, Color.red, 1f);
        Debug.DrawRay(attackCenter, Vector2.up * attackRange, Color.green, 1f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayer);
        Debug.Log("Hits found: " + hits.Length);
        Debug.Log("Enemy LayerMask value: " + enemyLayer.value);

        // Loop through all detected colliders
        foreach (Collider2D col in hits)
        {
            Debug.Log("Hit object: " + col.name);

            Health health = col.GetComponent<Health>();

            // Only apply damage if enemy was found
            if (health != null && !health.isPlayer)
            {
                Debug.Log("Applying damage to: " + health.gameObject.name);
                health.TakeDamage(attackDamage);
            }
        }
    }
}



// ***************  THIS IS THE END OF THE CODE  ************************


// ***************  KEY TERMS  ************************
//  variable
//  inspector
//  declaring
//  initializing
//  public
//  private
//  debug.log
//  string
//  float
//  integer (aka 'int')
//  GameObject
//  Input
//  KeyCode
//  string
//  Rigidbody2D
//  Vector2
//  Vector3
//  ||
//  &&
//  ++
//  *
//  ==
//  =
// !=






// ***************  IGNORE EVERYTHING BELOW THIS LINE!  ************************