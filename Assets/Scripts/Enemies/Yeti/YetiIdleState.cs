 using UnityEngine;

/// <summary>
/// Idle state for Yeti boss:
/// - Waits for a short duration
/// - Chooses next attack state based on player position and conditions
/// </summary>
public class YetiIdleState : EnemyState
{
    // ─────────────────────────────────────────────
    // Settings
    // ─────────────────────────────────────────────

    private const float waitTime = 2.0f;

    // ─────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────

    public YetiIdleState(
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
    }

    // ─────────────────────────────────────────────
    // State Logic
    // ─────────────────────────────────────────────

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (yeti.player == null)
        {
            return;
        }

        if (!HasWaitedEnough())
        {
            return;
        }

        EvaluateNextState();
    }

    // ─────────────────────────────────────────────
    // State Exit
    // ─────────────────────────────────────────────

    public override void Exit()
    {
        base.Exit();
    }

    // ─────────────────────────────────────────────
    // Decision Logic
    // ─────────────────────────────────────────────

    private bool HasWaitedEnough()
    {
        return Time.time >= startTime + waitTime;
    }

    private void EvaluateNextState()
    {
        if (ShouldEnterBreathState())
        {
            stateMachine.ChangeState(yeti.BreathState);
            return;
        }

        if (ShouldEnterSlamState())
        {
            stateMachine.ChangeState(yeti.SlamState);
            return;
        }

        // Default fallback
        stateMachine.ChangeState(yeti.BreathState);
    }

    private bool ShouldEnterBreathState()
    {
        return yeti.IsPlayerInBreathZone();
    }

    private bool ShouldEnterSlamState()
    {
        return yeti.useLocationForSlam &&
               yeti.slamStartPoint != null &&
               yeti.player.position.y > yeti.slamStartPoint.position.y;
    }
}