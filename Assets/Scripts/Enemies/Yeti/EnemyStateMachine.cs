using UnityEngine;
/// <summary>
/// Core state machine controller for enemy AI.
/// Handles safe state transitions and lifecycle management.
/// </summary>
public class EnemyStateMachine
{
    // ─────────────────────────────────────────────
    // Current State
    // ─────────────────────────────────────────────

    public EnemyState CurrentState { get; private set; }

    // ─────────────────────────────────────────────
    // Initialization
    // ─────────────────────────────────────────────

    public void InitializeStateMachine(EnemyState initialState)
    {
        if (initialState == null)
        {
            Debug.LogError("[EnemyStateMachine] Initial state is NULL.");
            return;
        }

        CurrentState = initialState;
        CurrentState.Enter();
    }

    // ─────────────────────────────────────────────
    // State Transition
    // ─────────────────────────────────────────────

    public void ChangeState(EnemyState newState)
    {
        if (newState == null)
        {
            Debug.LogWarning("[EnemyStateMachine] Attempted to change to NULL state.");
            return;
        }

        if (newState == CurrentState)
        {
            return; // prevent unnecessary re-entry
        }

        CurrentState?.Exit();

        CurrentState = newState;
        CurrentState.Enter();
    }

    // ─────────────────────────────────────────────
    // Update Helpers (optional safety layer)
    // ─────────────────────────────────────────────

    public void LogicUpdate()
    {
        CurrentState?.LogicUpdate();
    }

    public void PhysicsUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }
}