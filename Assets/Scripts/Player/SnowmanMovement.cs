using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Handles player movement, dash system, animation, and stun logic.
/// Clean separation from combat/health systems.
/// </summary>

public class SnowmanMovement : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Movement State
    // ─────────────────────────────────────────────

    public bool canMove = true;

    public bool isStunImmune = false;
    public float stunImmunityDuration = 2.0f;

    public float moveSpeed = 3f;

    // ─────────────────────────────────────────────
    // Dash
    // ─────────────────────────────────────────────

    [Header("Dash Settings")]
    [SerializeField] private float dashMultiplier = 3f;
    [SerializeField] private float dashDuration = 0.1f;
    [SerializeField] private float dashCooldown = 0.30f;

    private bool isDashing = false;
    private bool canDash = true;

    // ─────────────────────────────────────────────
    // Input
    // ─────────────────────────────────────────────

    [Header("Input Settings")]
    [SerializeField, Range(0.05f, 0.5f)]
    private float inputDeadzone = 0.2f;

    private InputAction moveAction;
    private InputAction dashAction;

    // ─────────────────────────────────────────────
    // Components
    // ─────────────────────────────────────────────

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // ─────────────────────────────────────────────
    // Shadow
    // ─────────────────────────────────────────────

    [Header("Shadow")]
    public Transform shadow;

    public Vector3 shadowScale = new Vector3(0.35f, 0.2f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float shadowAlpha = 0.6f;

    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Vector2 movement;
    public Vector2 lastDirection = Vector2.down;

    // ─────────────────────────────────────────────
    // Keys (FIXED naming consistency)
    // ─────────────────────────────────────────────

    public bool hasSolOdaKey = false;
    public bool hasUstOdaKey = false;
    public bool hasBossKey = false;
    public bool hasSonOdaKey = false;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CreateInputActions();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        dashAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        InitializeShadow();
    }

    private void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;
            return;
        }

        ReadMovement();
        UpdateAnimation();
        HandleDash();
        UpdateShadow();
    }

    private void FixedUpdate()
    {
        if (!canMove || rb == null) return;

        Vector2 dir = isDashing ? lastDirection.normalized : movement.normalized;

        float speed = isDashing ? moveSpeed * dashMultiplier : moveSpeed;

        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (sr == null) return;

        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    // ─────────────────────────────────────────────
    // Input
    // ─────────────────────────────────────────────

    private void CreateInputActions()
    {
        moveAction = new InputAction("Move", InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.AddBinding("<Gamepad>/dpad");

        dashAction = new InputAction("Dash", InputActionType.Button);
        dashAction.AddBinding("<Keyboard>/leftShift");
        dashAction.AddBinding("<Gamepad>/rightShoulder");
    }

    private void ReadMovement()
    {
        movement = moveAction.ReadValue<Vector2>();

        if (movement.sqrMagnitude < inputDeadzone * inputDeadzone)
            movement = Vector2.zero;
        else
            movement = Vector2.ClampMagnitude(movement, 1f);
    }

    private void HandleDash()
    {
        if (dashAction.WasPressedThisFrame() && canDash)
            StartCoroutine(DashRoutine());
    }

    // ─────────────────────────────────────────────
    // Animation
    // ─────────────────────────────────────────────

    private void UpdateAnimation()
    {
        if (anim == null) return;

        Vector2 animDir;

        if (movement.sqrMagnitude > 0f)
        {
            animDir = Mathf.Abs(movement.y) >= Mathf.Abs(movement.x)
                ? new Vector2(0f, Mathf.Sign(movement.y))
                : new Vector2(Mathf.Sign(movement.x), 0f);

            lastDirection = animDir;
        }
        else
        {
            animDir = lastDirection;
        }

        anim.SetFloat("MoveX", animDir.x);
        anim.SetFloat("MoveY", animDir.y);
        anim.SetBool("isMoving", movement.sqrMagnitude > 0f);
    }

    // ─────────────────────────────────────────────
    // Dash
    // ─────────────────────────────────────────────

    private IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    // ─────────────────────────────────────────────
    // Shadow
    // ─────────────────────────────────────────────

    private void InitializeShadow()
    {
        if (shadow == null) return;

        SpriteRenderer shadowSR = shadow.GetComponent<SpriteRenderer>();

        if (shadowSR != null)
        {
            shadowSR.color = new Color(0f, 0f, 0f, shadowAlpha);
            shadowSR.sortingLayerName = "Ground";
            shadowSR.sortingOrder = -1;
        }

        shadow.gameObject.SetActive(true);
    }

    private void UpdateShadow()
    {
        if (shadow != null)
            shadow.localScale = shadowScale;
    }

    // ─────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────

    public void IncreaseSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    // ─────────────────────────────────────────────
    // Stun 
    // ─────────────────────────────────────────────

    public IEnumerator StunRoutine(float duration)
    {
        if (isStunImmune) yield break;

        isStunImmune = true;
        canMove = false;

        yield return new WaitForSeconds(duration);

        canMove = true;

        yield return new WaitForSeconds(stunImmunityDuration);

        isStunImmune = false;
    }
}