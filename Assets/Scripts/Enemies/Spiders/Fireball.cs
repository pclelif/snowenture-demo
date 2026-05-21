using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a projectile system that moves in a fixed direction, applies damage on hit,
/// ignores its spawner, and handles lifetime-based self destruction. Supports directional
/// rotation and collision-based interaction with player and environment.
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class Fireball : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Projectile Settings
    // ─────────────────────────────────────────────

    [Header("Projectile Settings")]

    [SerializeField, Min(0f)]
    private float speed = 8f;

    [SerializeField, Min(0f)]
    private float damage = 10f;

    [SerializeField, Min(0f)]
    private float lifetime = 0.9f;

    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Vector2 direction;
    private GameObject spawner;
    private bool hasHit;

    private Collider2D projectileCollider;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        projectileCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        Move();
    }

    // ─────────────────────────────────────────────
    // Setup
    // ─────────────────────────────────────────────

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        if (direction == Vector2.zero)
            return;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }

    public void SetSpawner(GameObject owner)
    {
        spawner = owner;

        if (projectileCollider == null || spawner == null)
            return;

        Collider2D ownerCollider =
            spawner.GetComponent<Collider2D>();

        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, ownerCollider);
        }
    }

    // ─────────────────────────────────────────────
    // Movement
    // ─────────────────────────────────────────────

    private void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // ─────────────────────────────────────────────
    // Collision
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        if (other.gameObject == spawner)
            return;

        if (other.CompareTag("Enemy"))
            return;

        if (other.CompareTag("Player"))
        {
            HandlePlayerHit(other);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void HandlePlayerHit(Collider2D playerCollider)
    {
        PlayerHealth health =
            playerCollider.GetComponent<PlayerHealth>();

        if (health == null)
            return;

        hasHit = true;

        health.TakeDamage(damage);

        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    // Cleanup
    // ─────────────────────────────────────────────

    private void OnBecameInvisible()
    {
        if (gameObject.activeInHierarchy)
            Destroy(gameObject);
    }
}