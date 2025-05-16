using UnityEngine;

public class EnemyBubbleBobbleAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;

    public float playerDetectionRange = 3f;
    public Transform player;
    public float hopForceX = 2f;
    public float hopForceY = 5f;
    public float hopCooldown = 1.5f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool canHop = true;

    public int maxHealth = 3;
    private int currentHeealth;
    public bool isFinalPhase = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currentHeealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (CanSeePlayer() && CanChaseSafely())
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Ground check
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        // Wall check
        Vector2 wallCheckDir = movingRight ? Vector2.right : Vector2.left;
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, wallCheckDir, wallCheckDistance, groundLayer);

        if (!isGroundAhead || isWallAhead)
        {
            Flip();
        }
    }

    void ChasePlayer()
    {
        if (!canHop || player == null) return;

        canHop = false;

        Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;

        // Flip to face the player
        if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
            Flip();

        // Apply directional hop force toward the player
        rb.velocity = new Vector2(direction.x * hopForceX, hopForceY);

        // Reset hop after cooldown
        Invoke(nameof(ResetHop), hopCooldown);
    }

    void ResetHop()
    {
        canHop = true;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

        return distanceToPlayer <= playerDetectionRange && verticalDistance < 1.5f;
    }

    bool CanChaseSafely()
    {
        Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
        Vector2 checkPos = groundCheck.position + new Vector3(direction.x * 0.5f, 0, 0);
        return Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);

        if (wallCheck != null)
        {
            Vector3 dir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
    }
    public void TakeDamage(int damage, bool isCharged = false)
    {
        if (isFinalPhase && !isCharged)
        {
            Debug.Log("Enemy is invulnerable");
            return;
        }
        currentHeealth -= damage;
        Debug.Log("Enemy took " + damage + "health remaining" + currentHeealth);

        if (currentHeealth <=0)
        {
            Die();
        }
        else if(currentHeealth == 1 && !isFinalPhase)
        {
            enterFinalPhase();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy is dead");
        ScoreManager.Instance.AddScore(100);
        Destroy(gameObject);
    }

    private void enterFinalPhase()
    {
        isFinalPhase = true;
        Debug.Log("Enemy is invulnerable");
    }
}