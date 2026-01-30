using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private StateMachine<PlayerState> _stateMachine;
    private Player _player;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    private Rigidbody2D _rb;

    // State tracking
    private bool _wasGrounded;
    
    // Properties
    public PlayerState CurrentState => _stateMachine.CurrentState;
    public bool CanMove => CurrentState != PlayerState.Dashing;
    public bool CanJump => CurrentState != PlayerState.Dashing;
    public bool CanDash => CurrentState is PlayerState.Idle or PlayerState.Moving or PlayerState.Jumping or PlayerState.Falling;

    void Awake()
    {
        _player = GetComponent<Player>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        _rb = GetComponent<Rigidbody2D>();
        
        _stateMachine = new StateMachine<PlayerState>(PlayerState.Idle);
        SetupStates();
    }

    void Update()
    {
        _stateMachine.Update();
    }

    private void SetupStates()
    {
        // Idle State
        _stateMachine.AddState(PlayerState.Idle,
            enter: OnIdleEnter,
            update: OnIdleUpdate,
            exit: OnIdleExit
        );
        
        // Moving State
        _stateMachine.AddState(PlayerState.Moving,
            enter: OnMovingEnter,
            update: OnMovingUpdate,
            exit: OnMovingExit
        );
        
        // Jumping State
        _stateMachine.AddState(PlayerState.Jumping,
            enter: OnJumpingEnter,
            update: OnJumpingUpdate,
            exit: OnJumpingExit
        );
        
        // Falling State
        _stateMachine.AddState(PlayerState.Falling,
            enter: OnFallingEnter,
            update: OnFallingUpdate,
            exit: OnFallingExit
        );
        
        // Dashing State
        _stateMachine.AddState(PlayerState.Dashing,
            enter: OnDashingEnter,
            update: OnDashingUpdate,
            exit: OnDashingExit
        );
    }

    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState) return;
        
        var oldState = CurrentState;
        
        EventBus.Emit(PlayerActionEventType.OnStateExit, oldState);
        Debug.Log($"[PlayerState] [Emit Event] Exit: {oldState}");
        
        _stateMachine.ChangeState(newState);
        
        EventBus.Emit(PlayerActionEventType.OnStateEnter, newState);
        Debug.Log($"[PlayerState] [Emit Event] Enter: {newState}");
        
        EventBus.Emit(PlayerActionEventType.OnStateChanged, 
            new StateChangeEventData 
            { 
                FromState = oldState, 
                ToState = newState, 
                Timestamp = Time.time 
            });
        Debug.Log($"[PlayerState] [Emit Event] Changed: {oldState} → {newState}");
    }

    #region Idle State
    private void OnIdleEnter()
    {
        // Nothing specific needed for idle
    }

    private void OnIdleUpdate()
    {
        // Check for transitions
        if (_dash.IsDashing)
        {
            ChangeState(PlayerState.Dashing);
        }
        else if (_jump.IsJumping && !_movement.IsGrounded)
        {
            ChangeState(PlayerState.Jumping);
        }
        else if (!_movement.IsGrounded)
        {
            ChangeState(PlayerState.Falling);
        }
        else if (Mathf.Abs(_rb.velocity.x) > 0.1f)
        {
            ChangeState(PlayerState.Moving);
        }
    }

    private void OnIdleExit()
    {
    }
    #endregion

    #region Moving State
    private void OnMovingEnter()
    {
    }

    private void OnMovingUpdate()
    {
        // Check for transitions
        if (_dash.IsDashing)
        {
            ChangeState(PlayerState.Dashing);
        }
        else if (_jump.IsJumping && !_movement.IsGrounded)
        {
            ChangeState(PlayerState.Jumping);
        }
        else if (!_movement.IsGrounded)
        {
            ChangeState(PlayerState.Falling);
        }
        else if (Mathf.Abs(_rb.velocity.x) < 0.1f)
        {
            ChangeState(PlayerState.Idle);
        }
    }

    private void OnMovingExit()
    {
    }
    #endregion

    #region Jumping State
    private void OnJumpingEnter()
    {
    }

    private void OnJumpingUpdate()
    {
        // Check for transitions
        if (_dash.IsDashing)
        {
            ChangeState(PlayerState.Dashing);
        }
        else if (_rb.velocity.y < 0)
        {
            ChangeState(PlayerState.Falling);
        }
    }

    private void OnJumpingExit()
    {
    }
    #endregion

    #region Falling State
    private void OnFallingEnter()
    {
        _wasGrounded = false;
    }

    private void OnFallingUpdate()
    {
        // Check for transitions
        if (_dash.IsDashing)
        {
            ChangeState(PlayerState.Dashing);
        }
        else if (_jump.IsJumping && !_wasGrounded)
        {
            // Double jump
            ChangeState(PlayerState.Jumping);
        }
        else if (_movement.IsGrounded)
        {
            // Landed
            if (Mathf.Abs(_rb.velocity.x) > 0.1f)
            {
                ChangeState(PlayerState.Moving);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }
        
        _wasGrounded = _movement.IsGrounded;
    }

    private void OnFallingExit()
    {
    }
    #endregion

    #region Dashing State
    private void OnDashingEnter()
    {
    }

    private void OnDashingUpdate()
    {
        // Check for dash end
        if (!_dash.IsDashing)
        {
            // Dash ended, determine next state
            if (_movement.IsGrounded)
            {
                if (Mathf.Abs(_rb.velocity.x) > 0.1f)
                {
                    ChangeState(PlayerState.Moving);
                }
                else
                {
                    ChangeState(PlayerState.Idle);
                }
            }
            else
            {
                ChangeState(PlayerState.Falling);
            }
        }
    }

    private void OnDashingExit()
    {
    }
    #endregion
}
