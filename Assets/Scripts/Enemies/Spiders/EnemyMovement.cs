using UnityEngine;

/// <summary>
/// Handles enemy AI movement including player detection, chasing behavior,
/// smooth movement using velocity interpolation, and avoidance of overlapping
/// with nearby enemies. Also manages sprite facing direction and animation states.
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Movement Settings
    // ─────────────────────────────────────────────

    [Header("Movement Settings")]

    [SerializeField, Min(0f)]
    private float speed = 3f;

    [SerializeField, Min(0f)]
    private float stopDistance = 2f;

    [SerializeField, Min(0f)]
    private float detectionRange = 5f;

    [SerializeField, Min(0f)]
    private float acceleration = 10f;

    // ─────────────────────────────────────────────
    // Separation (Avoid overlap)
    // ─────────────────────────────────────────────

    [Header("Separation")]

    [SerializeField, Min(0f)]
    private float separationRadius = 1f;

    [SerializeField, Min(0f)]
    private float separationStrength = 1f;

    // ─────────────────────────────────────────────
    // Shadow
    // ─────────────────────────────────────────────

    [Header("Shadow")]

    [SerializeField]
    private Transform shadow;

    [SerializeField]
    private Vector3 shadowScale = new Vector3(0.35f, 0.2f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float shadowAlpha = 0.6f;

    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator anim;

    private Vector2 velocitySmooth;
    private bool isChasing;

    public Vector2 CurrentDirection { get; private set; }

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        CacheComponents();
        CachePlayer();
        InitializeShadow();
    }

    private void Update()
    {
        UpdateChaseState();
    }

    private void FixedUpdate()
    {
        if (PlayerHealth.IsDead || player == null || rb == null)
        {
            StopMovement();
            return;
        }

        if (!isChasing)
        {
            StopMovement();
            return;
        }

        MoveTowardsPlayer();
    }

    private void LateUpdate()
    {
        UpdateSortingOrder();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void CachePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    private void InitializeShadow()
    {
        if (shadow == null)
            return;

        SpriteRenderer shadowSR = shadow.GetComponent<SpriteRenderer>();

        if (shadowSR != null)
        {
            shadowSR.color = new Color(0f, 0f, 0f, shadowAlpha);
            shadowSR.sortingLayerName = "Ground";
            shadowSR.sortingOrder = -1;
        }

        shadow.gameObject.SetActive(true);
    }

    // ─────────────────────────────────────────────
    // AI State
    // ─────────────────────────────────────────────

    private void UpdateChaseState()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            isChasing = true;
    }

    // ─────────────────────────────────────────────
    // Movement
    // ─────────────────────────────────────────────

    private void MoveTowardsPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            SmoothStop();
            UpdateAnimation(false);
            return;
        }

        Vector2 direction =
            (player.position - transform.position).normalized;

        Vector2 targetVelocity =
            direction * speed;

        targetVelocity += CalculateSeparation();

        rb.linearVelocity = Vector2.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref velocitySmooth,
            0.15f
        );

        UpdateMovementState();
    }

    private void SmoothStop()
    {
        rb.linearVelocity = Vector2.Lerp(
            rb.linearVelocity,
            Vector2.zero,
            acceleration * Time.fixedDeltaTime
        );
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        UpdateAnimation(false);
    }

    // ─────────────────────────────────────────────
    // Separation System
    // ─────────────────────────────────────────────

    private Vector2 CalculateSeparation()
    {
        Vector2 separation = Vector2.zero;

        Collider2D[] neighbors =
            Physics2D.OverlapCircleAll(transform.position, separationRadius);

        foreach (var other in neighbors)
        {
            if (other == null || other.gameObject == gameObject)
                continue;

            if (!other.CompareTag("Enemy"))
                continue;

            Vector2 diff =
                (Vector2)transform.position - (Vector2)other.transform.position;

            separation += diff.normalized * separationStrength;
        }

        return separation;
    }

    // ─────────────────────────────────────────────
    // Movement State
    // ─────────────────────────────────────────────

    private void UpdateMovementState()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        UpdateAnimation(isMoving);

        if (isMoving)
        {
            CurrentDirection = rb.linearVelocity.normalized;

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = rb.linearVelocity.x < 0f;
            }
        }
    }

    // ─────────────────────────────────────────────
    // Animation
    // ─────────────────────────────────────────────

    private void UpdateAnimation(bool isMoving)
    {
        if (anim == null)
            return;

        anim.SetBool("isMoving", isMoving);
    }

    // ─────────────────────────────────────────────
    // Visual
    // ─────────────────────────────────────────────

    private void UpdateSortingOrder()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sortingOrder =
            Mathf.RoundToInt(-transform.position.y * 100);
    }
}