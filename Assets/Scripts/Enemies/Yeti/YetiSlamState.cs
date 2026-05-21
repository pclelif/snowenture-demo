using UnityEngine;
using System.Collections;

/// <summary>
/// Handles Yeti slam state:
/// - Icicle spawning pressure
/// - Arm slam attack cycles
/// - Slam zone detection
/// - State exit timing
/// </summary>
public class YetiSlamState : EnemyState
{
    // ─────────────────────────────────────────────
    // Runtime Coroutines
    // ─────────────────────────────────────────────

    private Coroutine icicleRoutine;
    private Coroutine armRoutine;

    private bool isArmSlamming;

    // ─────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────

    public YetiSlamState(
        Enemy yeti,
        string animationName,
        EnemyStateMachine stateMachine
    ) : base(yeti, animationName, stateMachine) { }

    // ─────────────────────────────────────────────
    // Enter
    // ─────────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();

        ResetHitboxes();

        if (yeti != null)
        {
            icicleRoutine = yeti.StartCoroutine(IcicleSpawnLoop());
            armRoutine = yeti.StartCoroutine(ArmSlamLoop());
        }
    }

    // ─────────────────────────────────────────────
    // Logic Update
    // ─────────────────────────────────────────────

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (ShouldExitState())
        {
            stateMachine.ChangeState(yeti.IdleState);
        }
    }

    // ─────────────────────────────────────────────
    // Exit
    // ─────────────────────────────────────────────

    public override void Exit()
    {
        StopCoroutines();

        ResetArmStates();
        ResetArmRotations();

        base.Exit();
    }

    // ─────────────────────────────────────────────
    // Core Loops
    // ─────────────────────────────────────────────

    private IEnumerator IcicleSpawnLoop()
    {
        while (true)
        {
            SpawnIcicle();
            yield return new WaitForSeconds(yeti.icicleSpawnInterval);
        }
    }

    private IEnumerator ArmSlamLoop()
    {
        while (true)
        {
            if (!IsPlayerInSlamZone())
            {
                yield return new WaitForSeconds(0.2f);
                continue;
            }

            isArmSlamming = true;

            ExecuteArmSlam();

            isArmSlamming = false;

            yield return new WaitForSeconds(0.5f);
        }
    }

    // ─────────────────────────────────────────────
    // Slam Execution
    // ─────────────────────────────────────────────

    private void ExecuteArmSlam()
    {
        if (yeti == null || yeti.player == null) return;

        bool isRightSide =
            yeti.player.position.x > yeti.transform.position.x;

        Transform arm =
            isRightSide ? yeti.rightArm : yeti.leftArm;

        GameObject hitbox =
            isRightSide ? yeti.rightSlamHitbox : yeti.leftSlamHitbox;

        Quaternion baseRot =
            isRightSide ? yeti.rightArmBaseRot : yeti.leftArmBaseRot;

        float windUpAngle =
            isRightSide ? -yeti.windUpAngle : yeti.windUpAngle;

        float strikeAngle =
            isRightSide ? yeti.strikeAngle : -yeti.strikeAngle;

        Quaternion windUpRot =
            baseRot * Quaternion.Euler(0, 0, windUpAngle);

        Quaternion strikeRot =
            baseRot * Quaternion.Euler(0, 0, strikeAngle);

        yeti.StartCoroutine(ArmSlamSequence(
            arm,
            hitbox,
            baseRot,
            windUpRot,
            strikeRot
        ));
    }

    private IEnumerator ArmSlamSequence(
        Transform arm,
        GameObject hitbox,
        Quaternion baseRot,
        Quaternion windUpRot,
        Quaternion strikeRot
    )
    {
        yield return RotateArm(arm, windUpRot, yeti.slamRaiseSpeed, EaseType.Smooth);

        yield return new WaitForSeconds(0.15f);

        SetArmDamageState(arm, true);

        yield return RotateArm(arm, strikeRot, yeti.slamDropSpeed, EaseType.Cubic);

        SetArmDamageState(arm, false);

        ActivateHitbox(hitbox);

        yield return new WaitForSeconds(0.1f);

        DeactivateHitbox(hitbox);

        yield return RotateArm(arm, baseRot, yeti.slamRecoverySpeed, EaseType.SmoothStep);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    private void SetArmDamageState(Transform arm, bool state)
    {
        if (arm == null) return;

        ContactDamage cd = arm.GetComponent<ContactDamage>();
        if (cd != null) cd.isAttacking = state;
    }

    private void ActivateHitbox(GameObject hitbox)
    {
        if (hitbox != null) hitbox.SetActive(true);
    }

    private void DeactivateHitbox(GameObject hitbox)
    {
        if (hitbox != null) hitbox.SetActive(false);
    }

    private void ResetHitboxes()
    {
        if (yeti.rightSlamHitbox != null)
            yeti.rightSlamHitbox.SetActive(false);

        if (yeti.leftSlamHitbox != null)
            yeti.leftSlamHitbox.SetActive(false);
    }

    private void ResetArmStates()
    {
        SetArmDamageState(yeti.leftArm, false);
        SetArmDamageState(yeti.rightArm, false);
    }

    private void ResetArmRotations()
    {
        if (yeti.leftArm != null)
            yeti.leftArm.localRotation = yeti.leftArmBaseRot;

        if (yeti.rightArm != null)
            yeti.rightArm.localRotation = yeti.rightArmBaseRot;
    }

    private void StopCoroutines()
    {
        if (icicleRoutine != null)
            yeti.StopCoroutine(icicleRoutine);

        if (armRoutine != null)
            yeti.StopCoroutine(armRoutine);
    }

    private bool ShouldExitState()
    {
        return Time.time >= startTime + yeti.slamStateDuration &&
               !isArmSlamming;
    }

    private bool IsPlayerInSlamZone()
    {
        if (yeti == null || yeti.player == null) return false;

        Vector2 p = yeti.player.position;
        Vector2 y = yeti.transform.position;

        float centerX = y.x + yeti.slamZoneXOffset;

        bool inX = Mathf.Abs(p.x - centerX) < yeti.slamZoneWidth;

        float top = y.y + yeti.slamZoneYOffset;
        float bottom = top - yeti.slamZoneHeight;

        return inX && (p.y < top && p.y > bottom);
    }

    private void SpawnIcicle()
    {
        if (yeti.player == null || yeti.iciclePrefab == null)
            return;

        Vector3 target = yeti.player.position;

        SnowmanMovement move =
            yeti.player.GetComponent<SnowmanMovement>();

        if (move != null && move.GetMovement().magnitude > 0.1f)
        {
            Vector2 dir = move.GetMovement().normalized;
            target += (Vector3)(dir * yeti.predictionDistance);
        }

        Object.Instantiate(yeti.iciclePrefab, target, Quaternion.identity);
    }

    // ─────────────────────────────────────────────
    // Rotation System
    // ─────────────────────────────────────────────

    private enum EaseType
    {
        Smooth,
        Cubic,
        SmoothStep
    }

    private IEnumerator RotateArm(
        Transform arm,
        Quaternion target,
        float duration,
        EaseType ease
    )
    {
        Quaternion start = arm.localRotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / duration);

            float e = ease switch
            {
                EaseType.Smooth => Mathf.Sin(n * Mathf.PI * 0.5f),
                EaseType.Cubic => n * n * n,
                _ => Mathf.SmoothStep(0, 1, n)
            };

            arm.localRotation = Quaternion.Slerp(start, target, e);
            yield return null;
        }

        arm.localRotation = target;
    }
}