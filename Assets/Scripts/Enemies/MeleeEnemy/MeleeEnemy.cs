using UnityEngine;
using System.Collections;

/// <summary>
/// Handles melee enemy AI:
/// - Player detection
/// - Movement & chasing
/// - Telegraph attack system
/// - Touch damage
/// - Animation control
/// - Shadow handling
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeEnemy : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    [Header("AI Settings")]

    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;

    // ─────────────────────────────────────────────
    // Animation / Aim
    // ─────────────────────────────────────────────

    [Header("Animation & Aiming")]
    [SerializeField] private Transform aimPoint;

    // ─────────────────────────────────────────────
    // Combat
    // ─────────────────────────────────────────────

    [Header("Combat")]

    [SerializeField] private Vector2 hitboxSize = new Vector2(1.5f, 1f);
    [SerializeField] private float hitboxDistance = 1f;
    [SerializeField] private float telegraphDelay = 1f;
    [SerializeField] private float warningDelay = 0.2f;

    [SerializeField] private int attackDamage = 10;
    [SerializeField] private GameObject attackConeVisual;

    // ─────────────────────────────────────────────
    // Touch Damage
    // ─────────────────────────────────────────────

    [Header("Touch Damage")]

    [SerializeField] private int touchDamage = 10;
    [SerializeField] private float touchDamageCooldown = 0.8f;

    private float lastTouchTime;

    // ─────────────────────────────────────────────
    // Shadow
    // ─────────────────────────────────────────────

    [Header("Shadow")]

    [SerializeField] private Transform shadow;

    [SerializeField] private Vector3 shadowScale = new Vector3(0.35f, 0.2f, 1f);

    [SerializeField, Range(0f, 1f)]
    private float shadowAlpha = 0.6f;

    // ─────────────────────────────────────────────
    // Components
    // ─────────────────────────────────────────────

    private Rigidbody2D rb;
    private Animator anim;

    // ─────────────────────────────────────────────
    // Runtime State
    // ─────────────────────────────────────────────

    private bool isAttacking;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CacheComponents();
        FindPlayerIfNeeded();
        InitializeAnimationDefaults();
    }

    private void Start()
    {
        InitializeShadow();
    }

    private void Update()
    {
        UpdateShadow();
    }

    private void FixedUpdate()
    {
        HandleAI();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void CacheComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    private void InitializeAnimationDefaults()
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", 1f);
        anim.SetFloat("MoveY", 0f);
    }

    // ─────────────────────────────────────────────
    // AI
    // ─────────────────────────────────────────────

    private void HandleAI()
    {
        if (ShouldStopAI())
        {
            StopMovement();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            StopMovement();
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;

        HandleMovement(dir);
        HandleAnimation(dir);

        if (distance <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackRoutine(dir));
        }
    }

    private bool ShouldStopAI()
    {
        return player == null || PlayerHealth.IsDead || isAttacking;
    }

    private void HandleMovement(Vector2 dir)
    {
        rb.linearVelocity = dir * moveSpeed;
    }

    private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
            anim.SetBool("isMoving", false);
    }

    // ─────────────────────────────────────────────
    // Animation
    // ─────────────────────────────────────────────

    private void HandleAnimation(Vector2 dir)
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", dir.x);
        anim.SetFloat("MoveY", dir.y);
        anim.SetBool("isMoving", true);

        if (aimPoint != null)
            aimPoint.up = dir;
    }

    // ─────────────────────────────────────────────
    // Attack System
    // ─────────────────────────────────────────────

    private IEnumerator AttackRoutine(Vector2 direction)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("isMoving", false);
            anim.SetFloat("MoveX", direction.x);
            anim.SetFloat("MoveY", direction.y);
            anim.SetTrigger("Attack");
        }

        if (attackConeVisual != null)
            attackConeVisual.SetActive(false);

        yield return new WaitForSeconds(warningDelay);

        if (PlayerHealth.IsDead)
        {
            isAttacking = false;
            yield break;
        }

        if (attackConeVisual != null)
            attackConeVisual.SetActive(true);

        Vector2 aimDir = aimPoint != null ? (Vector2)aimPoint.up : direction;

        Vector2 center = (Vector2)transform.position + aimDir * hitboxDistance;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, hitboxSize, aimPoint ? aimPoint.eulerAngles.z : 0f);

        foreach (var hit in hits)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();

            if (ph != null)
                ph.TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.15f);

        if (attackConeVisual != null)
            attackConeVisual.SetActive(false);

        yield return new WaitForSeconds(telegraphDelay);

        isAttacking = false;
    }

    // ─────────────────────────────────────────────
    // Touch Damage
    // ─────────────────────────────────────────────

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time < lastTouchTime + touchDamageCooldown) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();

        if (ph != null)
        {
            ph.TakeDamage(touchDamage);
            lastTouchTime = Time.time;
        }
    }

    // ─────────────────────────────────────────────
    // Shadow
    // ─────────────────────────────────────────────

    private void InitializeShadow()
    {
        if (shadow == null) return;

        SpriteRenderer sr = shadow.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.color = new Color(0f, 0f, 0f, shadowAlpha);
            sr.sortingLayerName = "Ground";
            sr.sortingOrder = -1;
        }

        shadow.gameObject.SetActive(true);
    }

    private void UpdateShadow()
    {
        if (shadow != null)
            shadow.localScale = shadowScale;
    }

    // ─────────────────────────────────────────────
    // Debug
    // ─────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;

        Vector2 dir = aimPoint != null ? (Vector2)aimPoint.up : Vector2.up;
        Vector2 center = (Vector2)transform.position + dir * hitboxDistance;

        Gizmos.DrawWireCube(center, hitboxSize);
    }
}