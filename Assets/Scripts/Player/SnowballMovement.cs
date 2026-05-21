using UnityEngine;

/// <summary>
/// Handles snowball projectile behavior:
/// - Movement
/// - Collision detection
/// - Damage application
/// - Cleanup when off-screen
/// </summary>
public sealed class SnowballMovement : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Movement Settings
    // ─────────────────────────────────────────────

    [Header("Movement")]

    [SerializeField, Min(0f)]
    private float speed = 5f;

    // ─────────────────────────────────────────────
    //  Combat Settings
    // ─────────────────────────────────────────────

    [Header("Combat")]

    [SerializeField, Min(0f)]
    private float damage = 10f;

    // ─────────────────────────────────────────────
    //  Runtime State
    // ─────────────────────────────────────────────

    private Vector2 direction;
    private bool hasHit;

    private PlayerAttack playerAttack;

    // ─────────────────────────────────────────────
    //  Cached Components
    // ─────────────────────────────────────────────

    private SpriteRenderer spriteRenderer;

    // ─────────────────────────────────────────────
    //  Public API
    // ─────────────────────────────────────────────

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetPlayerAttack(PlayerAttack attack)
    {
        playerAttack = attack;
    }

    // ─────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetupRenderOrder();
    }

    private void Update()
    {
        Move();
    }

    // ─────────────────────────────────────────────
    //  Movement
    // ─────────────────────────────────────────────

    private void Move()
    {
        transform.Translate(
            direction * speed * Time.deltaTime,
            Space.World
        );
    }

    // ─────────────────────────────────────────────
    //  Rendering
    // ─────────────────────────────────────────────

    private void SetupRenderOrder()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            return;
        }

        SpriteRenderer playerRenderer =
            player.GetComponent<SpriteRenderer>();

        if (playerRenderer == null)
        {
            return;
        }

        spriteRenderer.sortingLayerID =
            playerRenderer.sortingLayerID;

        spriteRenderer.sortingOrder =
            Mathf.RoundToInt(-transform.position.y * 100);
    }

    // ─────────────────────────────────────────────
    //  Collision
    // ─────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            return;
        }

        HandleCollision(other);
    }

    private void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleEnemyHit(other);
            return;
        }

        if (other.CompareTag("Scarecrow"))
        {
            HandleScarecrowHit(other);
            return;
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

    private void HandleEnemyHit(Collider2D other)
    {
        EnemyHealth enemyHealth =
            other.GetComponent<EnemyHealth>();

        YetiHealth yetiHealth =
            other.GetComponentInParent<YetiHealth>();

        if (enemyHealth == null && yetiHealth == null)
        {
            return;
        }

        hasHit = true;

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        else
        {
            yetiHealth.TakeDamage(Mathf.RoundToInt(damage));
        }

        playerAttack?.GainAttackPoints();

        Destroy(gameObject);
    }

    private void HandleScarecrowHit(Collider2D other)
    {
        Scarecrow scarecrow =
            other.GetComponent<Scarecrow>();

        if (scarecrow != null)
        {
            scarecrow.PlayHit();
            playerAttack?.GainAttackPoints();
        }

        Destroy(gameObject);
    }

    // ─────────────────────────────────────────────
    //  Cleanup
    // ─────────────────────────────────────────────

    private void OnBecameInvisible()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        Destroy(gameObject);
    }
}