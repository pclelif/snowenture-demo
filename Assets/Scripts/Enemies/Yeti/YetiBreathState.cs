using UnityEngine;

/// <summary>
/// Handles Yeti breath attack state logic:
/// - Activates breath hitbox
/// - Sweeps breath attack based on player position
/// - Returns to idle after duration
/// </summary>
public class YetiBreathState : EnemyState
{
    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    private readonly float attackDuration = 3.0f;

    private float sweepDirection;

    // ─────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────

    public YetiBreathState(
        Enemy yeti,
        string animationName,
        EnemyStateMachine stateMachine
    ) : base(yeti, animationName, stateMachine) { }

    // ─────────────────────────────────────────────
    // State Enter
    // ─────────────────────────────────────────────

    public override void Enter()
    {
        base.Enter();

        ActivateBreath(true);

        sweepDirection = CalculateSweepDirection();
    }

    // ─────────────────────────────────────────────
    // State Logic
    // ─────────────────────────────────────────────

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        float progress = (Time.time - startTime) / attackDuration;

        UpdateBreathRotation(progress);

        if (progress >= 1f)
        {
            stateMachine.ChangeState(yeti.IdleState);
        }
    }

    // ─────────────────────────────────────────────
    // State Exit
    // ─────────────────────────────────────────────

    public override void Exit()
    {
        ActivateBreath(false);

        ResetBreathRotation();

        base.Exit();
    }

    // ─────────────────────────────────────────────
    // Core Helpers
    // ─────────────────────────────────────────────

    private void ActivateBreath(bool state)
    {
        if (yeti.breathHitbox != null)
        {
            yeti.breathHitbox.SetActive(state);
        }
    }

    private float CalculateSweepDirection()
    {
        if (yeti.player == null)
        {
            return 1f;
        }

        return (yeti.player.position.x > yeti.transform.position.x)
            ? 1f
            : -1f;
    }

    private void UpdateBreathRotation(float progress)
    {
        if (yeti.breathPivot == null)
        {
            return;
        }

        float angle = Mathf.Lerp(
            60f * sweepDirection,
            -60f * sweepDirection,
            progress
        );

        yeti.breathPivot.transform.localRotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void ResetBreathRotation()
    {
        if (yeti.breathPivot != null)
        {
            yeti.breathPivot.transform.localRotation =
                Quaternion.identity;
        }
    }
}