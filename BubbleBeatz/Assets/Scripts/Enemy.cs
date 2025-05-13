using UnityEngine;

public class EnemyBubbleBobbleAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;
    public LayerMask groundLayer;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;

    private Rigidbody2D rb;
    private bool movingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Patrol();
    }

    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;

        // Move enemy
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Ground check
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);

        // Wall check (explicit left/right based on movement direction)
        Vector2 wallCheckDir = movingRight ? Vector2.right : Vector2.left;
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, wallCheckDir, wallCheckDistance, groundLayer);

        if (!isGroundAhead || isWallAhead)
        {
            Flip();
        }
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
    }
}