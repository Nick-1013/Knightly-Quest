using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Stats")]
    public int maxHealth = 3;
    public int damage = 1;
    public float attackCooldown = 2f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    [Header("References")]
    public Transform player;

    private Rigidbody2D rb;
    private Animator animator;
    private GameManagerScript gameManager;

    private EnemyState currentState;

    private float attackTimer;
    private bool isDead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindFirstObjectByType<GameManagerScript>();

        attackTimer = attackCooldown;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        currentState = EnemyState.Idle;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("Distance to player: " + distance + " | State: " + currentState);

        // Determine AI state
        if (distance <= attackRange)
            currentState = EnemyState.Attack;
        else if (distance <= detectionRange)
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.Idle;

        HandleAttackTimer();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                StopMoving();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Attack:
                StopMoving();
                break;
        }
    }

    void HandleAttackTimer()
    {
        if (currentState != EnemyState.Attack) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        transform.localScale = new Vector3(direction, 1, 1);

        if (animator != null)
            animator.SetBool("IsMoving", true);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (animator != null)
            animator.SetBool("IsMoving", false);
    }

    void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        if (player == null) return;

        Health playerHealth = player.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Enemy attacked player for " + damage + " damage");
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        StopMoving();

        if (animator != null)
            animator.SetTrigger("Die");

        if (gameManager != null)
            gameManager.EnemyKilled();

        Destroy(gameObject, 1.5f);
    }
}