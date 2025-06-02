using UnityEngine;
using System.Collections;

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
    NotePrefab projectile;
    public SpectrumAnalyzer spectrumm;

    private static int bounceChainMultiplier = 1;
    private static bool isChainKillActive = false;


    public int CurrentHealth => currentHeealth;

    public LayerMask enemyLayer;

    public Transform enemyCheck;
    public float enemyCheckDistance = 0.2f;

    public GameObject bubblePrefab;
    private bool isTrapped = false;

    public float deathBounceForceX = 3f;
    public float deathBounceForceY = 7f;
    public float deathBounceTorque = 100f; 
    public float deathCleanupDelay = 2f;

    private bool isDying = false;


    void Start()
    {
        spectrumm = FindAnyObjectByType<SpectrumAnalyzer>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currentHeealth = maxHealth;
    }

    void FixedUpdate()
    {
        if (isDying) return;

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

        // Enemy check
        Vector2 enemyCheckDir = movingRight ? Vector2.right : Vector2.left;
        bool isEnemyAhead = Physics2D.Raycast(enemyCheck.position, enemyCheckDir, enemyCheckDistance, enemyLayer);

        if (!isGroundAhead || isWallAhead || isEnemyAhead)
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

    private void TrapInBubble()
    {
        isTrapped = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (bubblePrefab)
        {
            Instantiate(bubblePrefab, transform.position, Quaternion.identity, transform);
        }

        Debug.Log("Enemy is trapped in a bubble. Needs a charged shot to die.");
    }


    public void TakeDamage(int damage, bool isCharged = false, bool isOnbeat = false)
    {
        if (isFinalPhase && !isCharged)
        {
            Debug.Log("Enemy is invulnerable");
            return;
        }

        currentHeealth -= damage;
        Debug.Log("Enemy took " + damage + ", health remaining: " + currentHeealth);

        if (currentHeealth <= 0)
        {
            if (!isTrapped && !isCharged)
            {
                TrapInBubble();
            }
            else
            {
                Die(isCharged, isOnbeat);
            }
        }
        else if (currentHeealth == 1 && !isFinalPhase)
        {
            enterFinalPhase();
        }
    }

    public IEnumerator BounceDeath(float bounceMultiplier = 1f)
    {
        isDying = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 1f;
        rb.freezeRotation = false;

        float direction = transform.position.x > player.position.x ? 1f : -1f;

        // Aggressive, chain-aware bounce
        float totalX = deathBounceForceX * 1.5f * bounceMultiplier;
        float totalY = deathBounceForceY * 1.5f * bounceMultiplier;
        float totalTorque = deathBounceTorque * 2f * bounceMultiplier;

        rb.velocity = new Vector2(direction * totalX, totalY);
        rb.AddTorque(direction * totalTorque);

        gameObject.layer = LayerMask.NameToLayer("BouncingEnemy");

        if (!isChainKillActive)
        {
            bounceChainMultiplier = 1;
            isChainKillActive = true;
        }

        yield return new WaitForSeconds(deathCleanupDelay);

        isChainKillActive = false;
        Destroy(gameObject);
    }

    private void Die(bool wasCharged, bool wasOnbeat)
    {
        if (!wasCharged)
        {
            Debug.LogWarning("Die() called without a charged shot! Shouldn't happen.");
            return;
        }

        int baseScore = 100;
        int finalScore = baseScore;

        if (isFinalPhase && wasOnbeat)
        {
            finalScore *= 3;
            Debug.Log("Triple points for killing final phase enemy on beat!");
        }

        ScoreManager.Instance.AddScore(finalScore);
        Debug.Log("Enemy is dead with score: " + finalScore);

        StartCoroutine(BounceDeath());
    }

    private void enterFinalPhase()
    {
        isFinalPhase = true;
        Debug.Log("Enemy is invulnerable");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDying || !isChainKillActive) return;

        EnemyBubbleBobbleAI other = collision.gameObject.GetComponent<EnemyBubbleBobbleAI>();

        if (other != null && !other.isDying && other != this)
        {
            bounceChainMultiplier *= 2;
            int chainScore = 100 * bounceChainMultiplier;
            ScoreManager.Instance.AddScore(chainScore);

            // Calculate bounce intensity based on chain length
            float bounceIntensity = Mathf.Log(bounceChainMultiplier, 2); // 1 for 2x, 2 for 4x, etc.
            other.DieFromBounce(bounceIntensity);
        }
    }

    public void DieFromBounce(float bounceMultiplier = 1f)
    {
        if (isDying) return;

        StartCoroutine(BounceDeath(bounceMultiplier));
    }


}