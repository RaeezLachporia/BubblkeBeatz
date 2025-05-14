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
    public float hopForce = 5f;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        if (CanSeePlayer())
        {
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Vector2 wallCheckDir = movingRight ? Vector2.right : Vector2.left;
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, wallCheckDir, wallCheckDistance, groundLayer);

        if (!isGroundAhead || isWallAhead)
        {
            Flip();
        }
    }

    void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;

            // Check ground ahead in chase direction
            Vector2 groundCheckPos = groundCheck.position + new Vector3(direction.x * 0.5f, 0, 0);
            bool groundAhead = Physics2D.Raycast(groundCheckPos, Vector2.down, groundCheckDistance, groundLayer);

            if (groundAhead)
            {
                // Flip to face player
                if ((direction.x > 0 && !movingRight) || (direction.x < 0 && movingRight))
                    Flip();

                // Small hop toward player
                rb.velocity = new Vector2(direction.x * moveSpeed, hopForce);
            }
            else
            {
                isChasing = false; // Prevent hopping off platform
            }
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

        return distanceToPlayer <= playerDetectionRange && verticalDistance < 1.5f; // vertical clamp to ensure "same platform"
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

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        }
    }
}