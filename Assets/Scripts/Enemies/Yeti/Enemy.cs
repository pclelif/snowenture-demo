using UnityEngine;

/// <summary>
/// Main enemy controller that handles state machine initialization,
/// arm references, attack zones, and player detection helpers.
/// </summary>
public class Enemy : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // State Machine
    // ─────────────────────────────────────────────

    public EnemyStateMachine StateMachine { get; private set; }

    public YetiIdleState IdleState { get; private set; }
    public YetiSlamState SlamState { get; private set; }
    public YetiBreathState BreathState { get; private set; }

    // ─────────────────────────────────────────────
    // Player Reference
    // ─────────────────────────────────────────────

    [Header("Player")]
    public Transform player;

    // ─────────────────────────────────────────────
    // Slam Settings
    // ─────────────────────────────────────────────

    [Header("Slam Start Location")]
    public Transform slamStartPoint;

    public bool useLocationForSlam = true;

    [Header("Slam Animation Settings")]
    public float windUpAngle = 45f;
    public float strikeAngle = 75f;
    public float slamRaiseSpeed = 0.6f;
    public float slamDropSpeed = 0.15f;
    public float slamRecoverySpeed = 0.3f;

    [Header("Slam Hitboxes")]
    public GameObject rightSlamHitbox;
    public GameObject leftSlamHitbox;

    [Header("Arm References")]
    public Transform leftArm;
    public Transform rightArm;

    [HideInInspector] public Quaternion leftArmBaseRot;
    [HideInInspector] public Quaternion rightArmBaseRot;

    // ─────────────────────────────────────────────
    // Attack Zones
    // ─────────────────────────────────────────────

    [Header("Breath Settings")]
    public GameObject breathHitbox;
    public GameObject breathPivot;

    [Header("Breath Zone Settings")]
    public Vector2 breathZoneSize = new Vector2(10f, 2f);
    public Vector2 breathZoneOffset = new Vector2(-0.85f, 2.26f);

    [Header("Slam Zone Settings")]
    public float slamZoneWidth = 12.7f;
    public float slamZoneHeight = 5.5f;
    public float slamZoneYOffset = 1f;
    public float slamZoneXOffset = -0.4f;

    [Header("Room Settings")]
    public float roomCenterY = 35f;

    // ─────────────────────────────────────────────
    // Icicle Attack
    // ─────────────────────────────────────────────

    [Header("Icicle Attack Settings")]
    public GameObject iciclePrefab;
    public float icicleSpawnInterval = 1.0f;
    public float predictionDistance = 2.0f;
    public float slamStateDuration = 4.0f;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        InitializeStateMachine();
        CacheArmRotations();
        CachePlayer();
    }

    private void Start()
    {
        StateMachine.InitializeStateMachine(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState?.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState?.PhysicsUpdate();
    }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    private void InitializeStateMachine()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new YetiIdleState(this, "Idle", StateMachine);
        SlamState = new YetiSlamState(this, "Slam", StateMachine);
        BreathState = new YetiBreathState(this, "Breath", StateMachine);
    }

    private void CacheArmRotations()
    {
        if (leftArm != null)
            leftArmBaseRot = leftArm.localRotation;

        if (rightArm != null)
            rightArmBaseRot = rightArm.localRotation;
    }

    private void CachePlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    public bool IsPlayerInBreathZone()
    {
        if (player == null) return false;

        Vector2 center = (Vector2)transform.position + breathZoneOffset;

        float dx = Mathf.Abs(player.position.x - center.x);
        float dy = Mathf.Abs(player.position.y - center.y);

        return dx <= breathZoneSize.x * 0.5f &&
               dy <= breathZoneSize.y * 0.5f;
    }

    // ─────────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        DrawBreathZoneGizmo();
        DrawSlamZoneGizmo();
    }

    private void DrawBreathZoneGizmo()
    {
        Gizmos.color = Color.cyan;

        Vector2 center = (Vector2)transform.position + breathZoneOffset;
        Gizmos.DrawWireCube(center, breathZoneSize);
    }

    private void DrawSlamZoneGizmo()
    {
        Gizmos.color = Color.green;

        float topBound = transform.position.y + slamZoneYOffset;
        float centerY = topBound - (slamZoneHeight * 0.5f);

        Vector3 centerPos = new Vector3(
            transform.position.x + slamZoneXOffset,
            centerY,
            0f
        );

        Vector3 size = new Vector3(
            slamZoneWidth * 2f,
            slamZoneHeight,
            0f
        );

        Gizmos.DrawWireCube(centerPos, size);
    }
}