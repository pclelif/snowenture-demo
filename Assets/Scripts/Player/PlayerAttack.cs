using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Handles all player combat functionality:
/// - Normal attacks
/// - Punch attacks
/// - Ultimate abilities
/// - Attack bar management
/// </summary>
[RequireComponent(typeof(SnowmanMovement))]
[RequireComponent(typeof(Collider2D))]
public sealed class PlayerAttack : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  Types
    // ─────────────────────────────────────────────

    private enum UltimateType
    {
        Spread,
        Heavy
    }

    // ─────────────────────────────────────────────
    //  Attack Bar
    // ─────────────────────────────────────────────

    [Header("Attack Bar")]

    [SerializeField]
    private GameObject attackBarObject;

    [SerializeField, Min(0f)]
    private float attackPoints = 0f;

    [SerializeField, Min(1f)]
    private float maxAttack = 100f;

    [SerializeField, Min(0f)]
    private float attackPerShot = 10f;

    // ─────────────────────────────────────────────
    //  Snowball Settings
    // ─────────────────────────────────────────────

    [Header("Snowball Settings")]

    [SerializeField]
    private GameObject snowballPrefab;

    [SerializeField, Min(0f)]
    private float normalShotCooldown = 0.4f;

    [SerializeField, Min(0f)]
    private float spreadUltimateCooldown = 0.5f;

    [SerializeField, Min(0f)]
    private float heavyUltimateCooldown = 0.5f;

    [SerializeField, Min(0f)]
    private float baseDamage = 10f;

    [SerializeField, Min(0f)]
    private float heavyUltimateDamageMultiplier = 3f;

    [SerializeField, Min(0f)]
    private float heavyUltimateScale = 4.5f;

    [SerializeField, Min(0f)]
    private float spreadAngle = 15f;

    [SerializeField, Range(0.5f, 3f)]
    private float normalShotScale = 1.5f;

    // ─────────────────────────────────────────────
    //  Punch Settings
    // ─────────────────────────────────────────────

    [Header("Punch Settings")]

    [SerializeField, Min(0f)]
    private float punchRange = 0.65f;

    [SerializeField, Min(0f)]
    private float punchRadius = 0.45f;

    [SerializeField, Min(0f)]
    private float punchCooldown = 0.35f;

    [SerializeField, Min(0f)]
    private float punchDamageMultiplier = 0.75f;

    [SerializeField, Min(0f)]
    private float punchMovementLockDuration = 0.2f;

    // ─────────────────────────────────────────────
    //  Debug Settings
    // ─────────────────────────────────────────────

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    [Header("Punch Debug")]

    [SerializeField]
    private bool showPunchDebug = true;

    [SerializeField]
    private Color punchDebugColor = Color.red;

    [SerializeField, Min(0f)]
    private float debugDuration = 0.15f;

    [SerializeField, Min(0.01f)]
    private float debugLineWidth = 0.04f;

    [SerializeField, Range(8, 64)]
    private int debugSegments = 40;

#endif

    // ─────────────────────────────────────────────
    //  Cached References
    // ─────────────────────────────────────────────

    private SnowmanMovement movement;
    private Animator animator;
    private Collider2D playerCollider;
    private AttackBarUI attackBarUI;

    // ─────────────────────────────────────────────
    //  Input
    // ─────────────────────────────────────────────

    private InputAction normalShotAction;
    private InputAction punchAction;
    private InputAction spreadUltimateAction;
    private InputAction heavyUltimateAction;

    // ─────────────────────────────────────────────
    //  Runtime State
    // ─────────────────────────────────────────────

    private float normalShotTimer;
    private float spreadUltimateTimer;
    private float heavyUltimateTimer;
    private float punchTimer;

    private bool canUseUltimate;
    private bool isInTrainingZone;

    // Avoids GC allocations during punch detection
    private static readonly Collider2D[] PunchHitBuffer = new Collider2D[16];

    // ─────────────────────────────────────────────
    //  Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        CacheComponents();
        CreateInputActions();
    }

    private void OnEnable()
    {
        EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void Update()
    {
        UpdateCooldownTimers();
        HandleCombatInput();
    }

    // ─────────────────────────────────────────────
    //  Initialization
    // ─────────────────────────────────────────────

    private void CacheComponents()
    {
        movement = GetComponent<SnowmanMovement>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        if (attackBarObject != null)
        {
            attackBarUI = attackBarObject.GetComponent<AttackBarUI>();
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        if (snowballPrefab == null)
        {
            Debug.LogWarning("[PlayerAttack] Snowball prefab is not assigned.");
        }

        if (attackBarObject == null)
        {
            Debug.LogWarning("[PlayerAttack] Attack bar object is missing.");
        }

#endif
    }

    private void CreateInputActions()
    {
        normalShotAction = CreateButtonAction(
            "Normal Shot",
            "<Mouse>/rightButton",
            "<Gamepad>/buttonWest"
        );

        punchAction = CreateButtonAction(
            "Punch",
            "<Mouse>/leftButton",
            "<Gamepad>/buttonNorth"
        );

        spreadUltimateAction = CreateButtonAction(
            "Spread Ultimate",
            "<Keyboard>/q",
            "<Gamepad>/leftShoulder"
        );

        heavyUltimateAction = CreateButtonAction(
            "Heavy Ultimate",
            "<Keyboard>/e",
            "<Gamepad>/rightShoulder"
        );
    }

    private static InputAction CreateButtonAction(
        string actionName,
        string primaryBinding,
        string secondaryBinding)
    {
        InputAction action = new InputAction(
            actionName,
            InputActionType.Button
        );

        action.AddBinding(primaryBinding);
        action.AddBinding(secondaryBinding);

        return action;
    }

    // ─────────────────────────────────────────────
    //  Input Handling
    // ─────────────────────────────────────────────

    private void EnableInput()
    {
        normalShotAction.Enable();
        punchAction.Enable();
        spreadUltimateAction.Enable();
        heavyUltimateAction.Enable();
    }

    private void DisableInput()
    {
        normalShotAction.Disable();
        punchAction.Disable();
        spreadUltimateAction.Disable();
        heavyUltimateAction.Disable();
    }

    private void HandleCombatInput()
    {
        if (IsPointerOverUI())
        {
            return;
        }

        HandleNormalShotInput();
        HandlePunchInput();
        HandleUltimateInput();
    }

    private void HandleNormalShotInput()
    {
        if (!normalShotAction.WasPressedThisFrame())
        {
            return;
        }

        if (normalShotTimer > 0f)
        {
            return;
        }

        PerformNormalShot();

        normalShotTimer = normalShotCooldown;
    }

    private void HandlePunchInput()
    {
        if (!punchAction.WasPressedThisFrame())
        {
            return;
        }

        TryPunch();
    }

    private void HandleUltimateInput()
    {
        if (!canUseUltimate)
        {
            return;
        }

        if (spreadUltimateAction.WasPressedThisFrame() &&
            spreadUltimateTimer <= 0f)
        {
            UseUltimate(UltimateType.Spread);

            spreadUltimateTimer = spreadUltimateCooldown;
        }
        else if (heavyUltimateAction.WasPressedThisFrame() &&
                 heavyUltimateTimer <= 0f)
        {
            UseUltimate(UltimateType.Heavy);

            heavyUltimateTimer = heavyUltimateCooldown;
        }
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    // ─────────────────────────────────────────────
    //  Cooldowns
    // ─────────────────────────────────────────────

    private void UpdateCooldownTimers()
    {
        float deltaTime = Time.deltaTime;

        ReduceTimer(ref normalShotTimer, deltaTime);
        ReduceTimer(ref spreadUltimateTimer, deltaTime);
        ReduceTimer(ref heavyUltimateTimer, deltaTime);
        ReduceTimer(ref punchTimer, deltaTime);
    }

    private static void ReduceTimer(ref float timer, float deltaTime)
    {
        if (timer <= 0f)
        {
            return;
        }

        timer -= deltaTime;

        if (timer < 0f)
        {
            timer = 0f;
        }
    }

    // ─────────────────────────────────────────────
    //  Normal Shot
    // ─────────────────────────────────────────────

    private void PerformNormalShot()
    {
        ShootSnowball(
            GetFacingDirection(),
            normalShotScale,
            1f
        );
    }

    // ─────────────────────────────────────────────
    //  Punch
    // ─────────────────────────────────────────────

    private void TryPunch()
    {
        if (punchTimer > 0f)
        {
            return;
        }

        punchTimer = punchCooldown;

        animator?.SetTrigger("Punch");

        StartCoroutine(LockMovementDuringPunch());

        PerformPunchHitDetection();
    }

    private void PerformPunchHitDetection()
    {
        Vector2 direction = GetFacingDirection();

        Vector2 hitCenter =
            (Vector2)transform.position +
            direction * punchRange;

#if UNITY_EDITOR || DEVELOPMENT_BUILD

        if (showPunchDebug)
        {
            DrawPunchDebug(hitCenter, punchRadius);
        }

#endif

        int hitCount = Physics2D.OverlapCircleNonAlloc(
            hitCenter,
            punchRadius,
            PunchHitBuffer
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D hit = PunchHitBuffer[i];

            if (hit == null || hit == playerCollider)
            {
                continue;
            }

            ProcessPunchHit(hit);

            PunchHitBuffer[i] = null;
        }
    }

    private void ProcessPunchHit(Collider2D hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            ApplyDamageToEnemy(hit);
            return;
        }

        if (hit.CompareTag("Scarecrow"))
        {
            HandleScarecrowHit(hit);
        }
    }

    private void ApplyDamageToEnemy(Collider2D hit)
    {
        float damage = baseDamage * punchDamageMultiplier;
        bool dealtDamage = false;

        EnemyHealth enemyHealth =
            hit.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            dealtDamage = true;
        }

        YetiHealth yetiHealth =
            hit.GetComponent<YetiHealth>();

        if (yetiHealth != null)
        {
            yetiHealth.TakeDamage(damage);
            dealtDamage = true;
        }

        if (dealtDamage)
        {
            GainAttackPoints();
        }
    }

    private void HandleScarecrowHit(Collider2D hit)
    {
        Scarecrow scarecrow =
            hit.GetComponent<Scarecrow>();

        if (scarecrow == null)
        {
            return;
        }

        scarecrow.PlayHit();

        GainAttackPoints();
    }

    private IEnumerator LockMovementDuringPunch()
    {
        if (movement == null)
        {
            yield break;
        }

        movement.enabled = false;

        yield return new WaitForSeconds(
            punchMovementLockDuration
        );

        movement.enabled = true;
    }

    // ─────────────────────────────────────────────
    //  Ultimate Abilities
    // ─────────────────────────────────────────────

    private void UseUltimate(UltimateType type)
    {
        Vector2 direction = GetFacingDirection();

        switch (type)
        {
            case UltimateType.Spread:
                FireSpreadUltimate(direction);
                break;

            case UltimateType.Heavy:
                FireHeavyUltimate(direction);
                break;
        }

        ConsumeUltimateCharge();
    }

    private void FireSpreadUltimate(Vector2 baseDirection)
    {
        for (int i = -1; i <= 1; i++)
        {
            Vector2 spreadDirection =
                Quaternion.Euler(
                    0f,
                    0f,
                    i * spreadAngle
                ) * baseDirection;

            ShootSnowball(
                spreadDirection,
                normalShotScale,
                1f
            );
        }
    }

    private void FireHeavyUltimate(Vector2 direction)
    {
        ShootSnowball(
            direction,
            heavyUltimateScale,
            heavyUltimateDamageMultiplier
        );
    }

    private void ConsumeUltimateCharge()
    {
        if (isInTrainingZone)
        {
            RefillUltimateImmediately();
            return;
        }

        attackPoints = 0f;
        canUseUltimate = false;

        attackBarUI?.ResetBar();
    }

    private void RefillUltimateImmediately()
    {
        attackPoints = maxAttack;
        canUseUltimate = true;

        attackBarUI?.UpdateAttack(
            attackPoints,
            maxAttack
        );
    }

    // ─────────────────────────────────────────────
    //  Snowball
    // ─────────────────────────────────────────────

    private void ShootSnowball(
        Vector2 direction,
        float scale,
        float damageMultiplier)
    {
        if (snowballPrefab == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning(
                "[PlayerAttack] Cannot shoot because snowball prefab is missing."
            );
#endif
            return;
        }

        if (direction.sqrMagnitude <= 0f)
        {
            direction = Vector2.down;
        }

        Vector2 spawnPosition =
            CalculateSpawnPosition(direction);

        GameObject snowball = Instantiate(
            snowballPrefab,
            spawnPosition,
            Quaternion.identity
        );

        ConfigureSnowball(
            snowball,
            direction,
            scale,
            damageMultiplier
        );

        IgnorePlayerCollision(snowball);
    }

    private void ConfigureSnowball(
        GameObject snowball,
        Vector2 direction,
        float scale,
        float damageMultiplier)
    {
        SnowballMovement snowballMovement =
            snowball.GetComponent<SnowballMovement>();

        if (snowballMovement == null)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogWarning(
                "[PlayerAttack] SnowballMovement component missing on snowball prefab."
            );
#endif
            return;
        }

        snowballMovement.SetDirection(direction);

        snowballMovement.transform.localScale =
            Vector3.one * scale;

        snowballMovement.SetPlayerAttack(this);

        snowballMovement.SetDamage(
            baseDamage * damageMultiplier
        );
    }

    private Vector2 CalculateSpawnPosition(Vector2 direction)
    {
        float offset =
            playerCollider != null
            ? playerCollider.bounds.extents.magnitude
            : 0.5f;

        return (Vector2)transform.position +
               direction.normalized * offset;
    }

    private void IgnorePlayerCollision(GameObject snowball)
    {
        if (playerCollider == null)
        {
            return;
        }

        Collider2D snowballCollider =
            snowball.GetComponent<Collider2D>();

        if (snowballCollider == null)
        {
            return;
        }

        Physics2D.IgnoreCollision(
            snowballCollider,
            playerCollider
        );
    }

    // ─────────────────────────────────────────────
    //  Attack Bar
    // ─────────────────────────────────────────────

    public void GainAttackPoints()
    {
        if (isInTrainingZone)
        {
            return;
        }

        attackPoints = Mathf.Clamp(
            attackPoints + attackPerShot,
            0f,
            maxAttack
        );

        attackBarUI?.UpdateAttack(
            attackPoints,
            maxAttack
        );

        if (attackPoints >= maxAttack)
        {
            canUseUltimate = true;

            attackBarUI?.Flash();
        }
    }

    public void RefillAttackBar()
    {
        attackPoints = maxAttack;
        canUseUltimate = true;

        attackBarUI?.UpdateAttack(
            attackPoints,
            maxAttack
        );

        attackBarUI?.Flash();
    }

    public bool IsAttackFull()
    {
        return attackPoints >= maxAttack;
    }

    // ─────────────────────────────────────────────
    //  Training Zone
    // ─────────────────────────────────────────────

    public void EnterTrainingZone()
    {
        isInTrainingZone = true;

        RefillUltimateImmediately();

        attackBarUI?.SetInfinityMode(true);
    }

    public void ExitTrainingZone()
    {
        isInTrainingZone = false;

        attackPoints = 0f;
        canUseUltimate = false;

        attackBarUI?.SetInfinityMode(false);
        attackBarUI?.ResetBar();
    }

    // ─────────────────────────────────────────────
    //  Upgrades
    // ─────────────────────────────────────────────

    public void ApplyPermanentUpgrade(float amount)
    {
        baseDamage += amount;
    }

    public void IncreaseAttackPerShot(float amount)
    {
        attackPerShot += amount;
    }

    // ─────────────────────────────────────────────
    //  Utility
    // ─────────────────────────────────────────────

    private Vector2 GetFacingDirection()
    {
        if (movement != null &&
            movement.lastDirection.sqrMagnitude > 0f)
        {
            return movement.lastDirection.normalized;
        }

        return Vector2.down;
    }

    // ─────────────────────────────────────────────
    //  Debug Rendering
    // ─────────────────────────────────────────────

#if UNITY_EDITOR || DEVELOPMENT_BUILD

    private void DrawPunchDebug(
        Vector2 center,
        float radius)
    {
        if (debugSegments <= 2)
        {
            return;
        }

        GameObject debugObject =
            new GameObject("Punch Debug");

        LineRenderer lineRenderer =
            debugObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.positionCount = debugSegments;

        lineRenderer.startWidth = debugLineWidth;
        lineRenderer.endWidth = debugLineWidth;

        lineRenderer.startColor = punchDebugColor;
        lineRenderer.endColor = punchDebugColor;

        lineRenderer.sortingOrder = 999;

        lineRenderer.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        float angleStep =
            Mathf.PI * 2f / debugSegments;

        for (int i = 0; i < debugSegments; i++)
        {
            float angle = i * angleStep;

            Vector3 point = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y + Mathf.Sin(angle) * radius,
                0f
            );

            lineRenderer.SetPosition(i, point);
        }

        Destroy(debugObject, debugDuration);
    }

#endif
}