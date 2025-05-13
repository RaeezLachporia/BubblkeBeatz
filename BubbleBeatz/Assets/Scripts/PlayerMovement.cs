using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Shooting")]
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float noteCooldown = 0.3f;
    [SerializeField] private float shootBufferTime = 0.2f;

    [Header("Jump Buffer & Coyote Time")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    private Rigidbody2D rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private float shootBufferCounter;
    private float nextFireTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.Gameplay.Jump.performed += ctx => jumpBufferCounter = jumpBufferTime;
        inputActions.Gameplay.Shoot.performed += ctx => shootBufferCounter = shootBufferTime;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // Movement input
        moveInput = inputActions.Gameplay.Right.ReadValue<float>() > 0
            ? Vector2.right
            : inputActions.Gameplay.Left.ReadValue<float>() > 0
                ? Vector2.left
                : Vector2.zero;

        // Flip sprite
        if (moveInput.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Timers
        jumpBufferCounter -= Time.deltaTime;
        shootBufferCounter -= Time.deltaTime;

        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }

        // Gravity adjustments
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !inputActions.Gameplay.Jump.ReadValue<float>().Equals(1f))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Shoot with buffer
        if (shootBufferCounter > 0 && Time.time >= nextFireTime)
        {
            ShootNote();
            nextFireTime = Time.time + noteCooldown;
            shootBufferCounter = 0f;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
    }

    private void ShootNote()
    {
        GameObject note = Instantiate(notePrefab, firePoint.position, Quaternion.identity);
        NotePrefab projectile = note.GetComponent<NotePrefab>();
        projectile.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }
}