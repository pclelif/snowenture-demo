using UnityEngine;

/// <summary>
/// Controls ranged enemy attack behavior including detecting player within range,
/// firing projectiles at a fixed attack rate, and handling projectile instantiation
/// and initialization (direction + spawner reference).
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Attack Settings
    // ─────────────────────────────────────────────

    [Header("Attack Settings")]

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField, Min(0.1f)]
    private float attackRate = 0.7f;

    [SerializeField, Min(0f)]
    private float attackRange = 5f;

    // ─────────────────────────────────────────────
    // Runtime
    // ─────────────────────────────────────────────

    private Transform player;
    private float nextAttackTime;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CachePlayer();
    }

    private void Update()
    {
        if (!CanAttack())
            return;

        TryAttack();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void CachePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    // ─────────────────────────────────────────────
    // Attack Logic
    // ─────────────────────────────────────────────

    private bool CanAttack()
    {
        if (player == null)
            return false;

        if (PlayerHealth.IsDead)
            return false;

        float distance = Vector2.Distance(transform.position, player.position);

        return distance <= attackRange;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        Shoot();

        nextAttackTime = Time.time + attackRate;
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 direction =
            (player.position - firePoint.position).normalized;

        GameObject projectile =
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Fireball fireball = projectile.GetComponent<Fireball>();

        if (fireball != null)
        {
            fireball.SetDirection(direction);
            fireball.SetSpawner(gameObject);
        }
    }

    // ─────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}