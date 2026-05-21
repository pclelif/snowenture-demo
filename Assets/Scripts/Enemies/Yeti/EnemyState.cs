using UnityEngine;

/// <summary>
/// Base class for all enemy states in the state machine system.
/// Handles animation flags, timing, and transition control.
/// </summary>
public abstract class EnemyState
{
    // ─────────────────────────────────────────────
    // Core References
    // ─────────────────────────────────────────────
    protected Enemy yeti;
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;
    protected Animator animator;

    protected string animationBoolName;

    // ─────────────────────────────────────────────
    // State Runtime Data
    // ─────────────────────────────────────────────

    protected bool isExitingState;
    protected bool isAnimationFinished;
    protected float startTime;

    // ─────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────

    protected EnemyState(
        Enemy enemy,
        string animationBoolName,
        EnemyStateMachine stateMachine)
    {
        this.enemy = enemy;
        this.animationBoolName = animationBoolName;
        this.stateMachine = stateMachine;

        if (enemy != null)
            animator = enemy.GetComponent<Animator>();
    }

    // ─────────────────────────────────────────────
    // State Lifecycle
    // ─────────────────────────────────────────────

    public virtual void Enter()
    {
        isExitingState = false;
        isAnimationFinished = false;
        startTime = Time.time;

        SetAnimationBool(true);
    }

    public virtual void Exit()
    {
        isExitingState = true;

        SetAnimationBool(false);
    }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void TransitionChecks() { }

    // ─────────────────────────────────────────────
    // Animation Events
    // ─────────────────────────────────────────────

    public virtual void AnimationTrigger()
    {
        isAnimationFinished = true;
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    protected void SetAnimationBool(bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(animationBoolName))
        {
            animator.SetBool(animationBoolName, value);
        }
    }

    protected bool IsAnimationFinished()
    {
        return isAnimationFinished;
    }

    protected float GetStateTime()
    {
        return Time.time - startTime;
    }
}