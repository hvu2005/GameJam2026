
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMovement))]
public class PlayerJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;

    private PlayerInput _input;
    private Rigidbody2D _rb;
    private PlayerMovement _movement;
    private PlayerDash _dash;
    private PlayerStatusEffects _statusEffects;
    private PlayerFormController _formController;
    private PlayerGravityController _gravityController;
    private PlayerStateMachine _stateMachine;

    private int _jumpCount;
    private float _lastGroundedTime = -999f;
    private float _lastJumpTime = -999f;
    private float _previousVelocityY;

    private bool _isJumping;
    private bool _wasGrounded;

    public bool IsJumping => _isJumping;
    public int JumpCount => _jumpCount;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement>();
        _dash = GetComponent<PlayerDash>();
        _statusEffects = GetComponent<PlayerStatusEffects>();
        _formController = GetComponent<PlayerFormController>();
        _gravityController = GetComponent<PlayerGravityController>();
        _stateMachine = GetComponent<PlayerStateMachine>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig is not assigned to PlayerJump!", this);
        }
    }

    void OnEnable()
    {
        _input.On<bool>(PlayerInputType.Jump, OnJumpInput);
    }

    void OnDisable()
    {
        _input.Off<bool>(PlayerInputType.Jump, OnJumpInput);
    }

    void Update()
    {
        if (config == null) return;
        
        UpdateJumpTimers();
        ApplyGravityModifiers();
        CheckJumpPeak();

        if(_rb.velocity.y < -0.1f && !_movement.IsGrounded)
        {
            // if (_stateMachine != null && _stateMachine.CurrentState != PlayerState.Falling)
            // {
            //     _stateMachine.ChangeState(PlayerState.Falling);
            // }
            EventBus.Emit(PlayerActionEventType.OnFalling,
                new JumpEventData
                {
                    JumpCount = _jumpCount,
                    IsDoubleJump = _jumpCount > 1,
                    JumpForce = 0f,
                    UsedCoyoteTime = false,
                    Position = transform.position
                });
        }
    }

    private void OnJumpInput(bool pressed)
    {
        if (!pressed) return;
        
        if (_stateMachine != null && !_stateMachine.CanJump)
        {
            return;
        }
        
        if (_dash != null && _dash.IsDashing)
        {
            _dash.CancelDash();
        }
        
        if (CanJump())
        {
            PerformJump();
        }
    }

    private void UpdateJumpTimers()
    {
        bool isJumpCooldown = Time.time < _lastJumpTime + 0.1f;

        if (_movement.IsGrounded)
        {
            _lastGroundedTime = Time.time;

            if (!isJumpCooldown && (_jumpCount > 0 || _isJumping))
            {
                _jumpCount = 0;
                _isJumping = false;
            }
        }

        if (!_wasGrounded && _movement.IsGrounded && !isJumpCooldown)
        {
            OnLanded();
        }

        _wasGrounded = _movement.IsGrounded;
    }

    private bool CanJump()
    {
        if (config == null) return false;

        int maxJumps = _formController != null && _formController.CurrentForm != null 
            ? _formController.CurrentForm.GetMaxJumps() 
            : 1;

        bool withinCoyoteTime = Time.time - _lastGroundedTime < config.CoyoteTime;
        bool hasGroundJump = (_movement.IsGrounded || withinCoyoteTime) && _jumpCount == 0;

        if (hasGroundJump)
        {
            return true;
        }
        
        if (!_movement.IsGrounded && _jumpCount < maxJumps)
        {
            return true;
        }

        return false;
    }

    private void PerformJump()
    {
        if (config == null) return;

        bool withinCoyoteTime = Time.time - _lastGroundedTime < config.CoyoteTime;
        bool usedCoyoteTime = withinCoyoteTime && !_movement.IsGrounded;
        bool isDoubleJump = _jumpCount > 0;

        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        
        float jumpForce = _jumpCount == 0 ? config.JumpForce : config.DoubleJumpForce;
        float multiplier = _statusEffects != null ? _statusEffects.JumpForceMultiplier : 1f;
        jumpForce *= multiplier;
        
        int jumpDirection = _gravityController != null && _gravityController.IsGravityFlipped
            ? -1
            : 1;
        
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce * jumpDirection);

        EventBus.Emit(PlayerActionEventType.OnJumpStarted,
            new JumpEventData
            {
                JumpCount = _jumpCount + 1,
                IsDoubleJump = isDoubleJump,
                JumpForce = jumpForce,
                UsedCoyoteTime = usedCoyoteTime,
                Position = transform.position
            });
        Debug.Log($"[Jump] [Emit Event] Jump Started: Count={_jumpCount + 1}, Force={jumpForce:F1}");
        
        if (isDoubleJump)
        {
            EventBus.Emit(PlayerActionEventType.OnDoubleJump,
                new JumpEventData
                {
                    JumpCount = _jumpCount + 1,
                    IsDoubleJump = true,
                    JumpForce = jumpForce,
                    UsedCoyoteTime = false,
                    Position = transform.position
                });
            Debug.Log($"[Jump] [Emit Event] Double Jump! Count={_jumpCount + 1}");
        }
        
        if (usedCoyoteTime)
        {
            EventBus.Emit(PlayerActionEventType.OnCoyoteTimeUsed,
                new JumpEventData
                {
                    JumpCount = 1,
                    IsDoubleJump = false,
                    JumpForce = jumpForce,
                    UsedCoyoteTime = true,
                    Position = transform.position
                });
            Debug.Log("[Jump] [Emit Event] Coyote Time Used!");
        }

        _jumpCount++;
        _isJumping = true;
        _lastJumpTime = Time.time;
        
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(PlayerState.Jumping);
        }
    }

    private void ApplyGravityModifiers()
    {
        if (config == null) return;
        
        bool isFalling = _gravityController != null && _gravityController.IsGravityFlipped
            ? _rb.velocity.y > 0
            : _rb.velocity.y < 0;

        if (isFalling)
        {
            int gravityDir = _gravityController != null ? _gravityController.GravityDirection : 1;
            _rb.velocity += Vector2.up * Physics2D.gravity.y * gravityDir * (config.FallGravityMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnLanded()
    {
        EventBus.Emit(PlayerActionEventType.OnLanded,
            new JumpEventData
            {
                JumpCount = _jumpCount,
                IsDoubleJump = _jumpCount > 1,
                JumpForce = 0f,
                UsedCoyoteTime = false,
                Position = transform.position
            });
        Debug.Log($"[Jump] [Emit Event] Landed! JumpCount was: {_jumpCount}");
        
        _isJumping = false;
    }
    
    private void CheckJumpPeak()
    {
        if (_isJumping)
        {
            bool wasGoingUp = _previousVelocityY > 0.1f;
            bool nowGoingDown = _rb.velocity.y < 0.1f;
            
            if (wasGoingUp && nowGoingDown)
            {
                EventBus.Emit(PlayerActionEventType.OnJumpPeak,
                    new JumpEventData
                    {
                        JumpCount = _jumpCount,
                        IsDoubleJump = _jumpCount > 1,
                        JumpForce = 0f,
                        UsedCoyoteTime = false,
                        Position = transform.position
                    });
                Debug.Log("[Jump] [Emit Event] Peak Reached!");
            }
        }
        
        _previousVelocityY = _rb.velocity.y;
    }

    public void CancelJump()
    {
        if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        }
        _isJumping = false;
    }

    public void ResetJumpCount()
    {
        _jumpCount = 0;
        _isJumping = false;
    }
}
