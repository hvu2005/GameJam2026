
public enum PlayerActionEventType
{
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
    
    // State Events
    OnStateChanged,
    OnStateEnter,
    OnStateExit
}
