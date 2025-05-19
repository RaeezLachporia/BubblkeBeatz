using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private AnimationCurve chargeScale;


    [Header("Jump Buffer & Coyote Time")]
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;

    [Header("Dash variables")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimeRemaining;
    private float lastDashTime = Mathf.NegativeInfinity;
    private Vector2 dashDirection;

    [Header("Charged shot variables")]
    private GameObject chargedNote;
    [SerializeField]private float maxChargTime = 2f;
    [SerializeField] private float maxScaleMultiplier = 2f;
    private bool isCharging;
    private float chargeTime;
    private Vector3 originalNoteSize;

    [Header("Beat Manager variables for onBeat hits")]
    private SpectrumAnalyzer spectrumizer;
    [Header("generic variable")]
    [SerializeField] private Slider chargeSlider;
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
        inputActions.Gameplay.Dash.performed += ctx => TryDash();
        inputActions.Gameplay.ChargedShot.started += ctx => StartCharging();
        inputActions.Gameplay.ChargedShot.canceled += ctx => ReleaseCharged();
        spectrumizer = FindAnyObjectByType<SpectrumAnalyzer>();
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
        if (isCharging&&chargedNote!= null)
        {
            chargeTime += Time.deltaTime;
            float t = Mathf.Clamp01(chargeTime / maxChargeTime);
            float scale = Mathf.Lerp(1f, maxScaleMultiplier, t);
            chargedNote.transform.localScale = originalNoteSize * scale;

            chargedNote.transform.position = firePoint.position;

            chargeSlider.value = t;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
        if (isDashing)
        {
            rb.velocity = new Vector2(moveInput.normalized.x * dashSpeed, 0f);
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining<=0f)
            {
                isDashing = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(moveInput.x * speed, rb.velocity.y);
        }
    }

    private void ShootNote()
    {
        GameObject note = Instantiate(notePrefab, firePoint.position, Quaternion.identity);
        NotePrefab projectile = note.GetComponent<NotePrefab>();
        projectile.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    }

    private void TryDash()
    {
        if (Time.time >= lastDashTime + dashCooldown&& !isDashing&&moveInput != Vector2.zero)
        {
            isDashing = true;
            dashTimeRemaining = dashDuration;
            lastDashTime = Time.time;
        }
    }
    

    private void StartCharging()
    {
        chargeSlider.gameObject.SetActive(true);
        chargeSlider.value = 0f;
        if (chargedNote != null)
        {
            return;
        }
        chargedNote = Instantiate(notePrefab, firePoint.position, Quaternion.identity);
        originalNoteSize = chargedNote.transform.localScale;
        chargeTime = 0f;
        isCharging = true;
    }
    private void ReleaseCharged()
    {
        if (chargedNote == null) return;
        isCharging = false;
        bool onBeat = !spectrumizer != null && spectrumizer.IsBeatDetected();
        
        NotePrefab projectile = chargedNote.GetComponent<NotePrefab>();
        projectile.direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        projectile.isCharged = true;
        chargedNote = null;
        chargeTime = 0f;
        chargeSlider.gameObject.SetActive(false);
        if (onBeat)
        {
            projectile.isOnBeat = true;
            Debug.Log("Shot is on beat ");
        }
        else
        {
            projectile.isOnBeat = false;
            Debug.Log("Shot is not on beat");
        }
    }
}