
public enum PlayerActionEventType
{
    OnDie,
    // Movement Events
    OnMoveStarted,
    OnMoveStopped,
    OnDirectionChanged,
    OnSpeedChanged,
    
    // Jump Events
    OnJumpStarted,
    OnJumpPeak,
    OnLanded,
    OnDoubleJump,
    OnCoyoteTimeUsed,
    
    // Dash Events
    OnDashStarted,
    OnDashEnded,
    OnDashCooldownStart,
    OnDashReady,

    OnFalling,
    
    // State Events
    OnStateChanged,
    OnStateEnter,
    OnStateExit
}
