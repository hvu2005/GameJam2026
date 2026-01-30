
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;
    
    private PlayerInput _input;
    private Rigidbody2D _rb;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerStateMachine _stateMachine;
    private PlayerGravityController _gravityController;
    
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private float _previousCooldownTimer;
    private Vector2 _dashDirection;
    private Vector3 _dashStartPosition;
    private float _originalGravityScale;
    private bool _hasLandedSinceDash = true;

    public bool IsDashing => _isDashing;
    public bool CanDash => _dashCooldownTimer <= 0f && (!_isDashing) && _hasLandedSinceDash;
    public float DashCooldownPercent => Mathf.Clamp01(1f - (_dashCooldownTimer / config.DashCooldown));

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb =GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _stateMachine = GetComponent<PlayerStateMachine>();
        _gravityController = GetComponent<PlayerGravityController>();
        _originalGravityScale = Mathf.Abs(_rb.gravityScale);
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig is not assigned to PlayerDash!", this);
        }
    }

    void OnEnable()
    {
        _input.On<bool>(PlayerInputType.Dash, OnDashInput);
    }

    void OnDisable()
    {
        _input.Off<bool>(PlayerInputType.Dash, OnDashInput);
    }

    void Update()
    {
        UpdateDashState();
        UpdateCooldown();

        if (_movement.IsGrounded && !_isDashing)
        {
            _hasLandedSinceDash = true;
        }
    }

    private void OnDashInput(bool pressed)
    {
        if (!pressed) return;
        
        if (_stateMachine != null && !_stateMachine.CanDash)
        {
            return;
        }
        
        if (CanPerformDash())
        {
            StartDash();
        }
    }

    private bool CanPerformDash()
    {
        if (config == null) return false;
        if (!CanDash) return false;
        
        if (!_movement.IsGrounded && !config.CanDashInAir)
        {
            return false;
        }
        
        return true;
    }

    private void StartDash()
    {
        if (config == null) return;

        _isDashing = true;
        _dashTimer = config.DashDuration;
        _dashCooldownTimer = config.DashCooldown;
        _hasLandedSinceDash = false;
        
        _dashDirection = new Vector2(_movement.FacingDirection, 0f);
        _dashStartPosition = transform.position;
        
        _rb.gravityScale = 0f;
        _rb.velocity = _dashDirection * config.DashSpeed;
        
        EventBus.Emit(PlayerActionEventType.OnDashStarted,
            new DashEventData
            {
                DashDirection = _dashDirection,
                DashSpeed = config.DashSpeed,
                CooldownRemaining = config.DashCooldown,
                StartPosition = _dashStartPosition
            });
        Debug.Log($"[Dash] [Emit Event] Dash Started: Dir={_dashDirection}, Speed={config.DashSpeed}");
        
        EventBus.Emit(PlayerActionEventType.OnDashCooldownStart, config.DashCooldown);
        Debug.Log($"[Dash] [Emit Event] Cooldown Started: {config.DashCooldown}s");
        
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(PlayerState.Dashing);
        }
    }

    private void UpdateDashState()
    {
        if (!_isDashing) return;
        if (config == null) return;

        _dashTimer -= Time.deltaTime;
        
        _rb.velocity = _dashDirection * config.DashSpeed;
        
        if (_dashTimer <= 0f)
        {
            EndDash();
        }
    }

    private void EndDash()
    {
        _isDashing = false;
        
        // Restore gravity with correct direction if gravity was flipped during dash
        if (_gravityController != null)
        {
            _rb.gravityScale = _gravityController.GetCurrentGravityScale(_originalGravityScale);
        }
        else
        {
            _rb.gravityScale = _originalGravityScale;
        }

        float newVelocityX = _rb.velocity.x * 0.5f;
        float newVelocityY = _dashDirection.y >= 0 ? 0f : _rb.velocity.y;
        _rb.velocity = new Vector2(newVelocityX, newVelocityY);
        
        EventBus.Emit(PlayerActionEventType.OnDashEnded,
            new DashEventData
            {
                DashDirection = _dashDirection,
                DashSpeed = 0f,
                CooldownRemaining = _dashCooldownTimer,
                StartPosition = _dashStartPosition
            });
        Debug.Log($"[Dash] [Emit Event] Dash Ended. Cooldown remaining: {_dashCooldownTimer:F1}s");
    }

    private void UpdateCooldown()
    {
        if (_dashCooldownTimer > 0f)
        {
            _previousCooldownTimer = _dashCooldownTimer;
            _dashCooldownTimer -= Time.deltaTime;
            
            if (_previousCooldownTimer > 0f && _dashCooldownTimer <= 0f)
            {
                EventBus.Emit(PlayerActionEventType.OnDashReady, true);
                Debug.Log("[Dash] [Emit Event] Dash Ready!");
            }
        }
    }

    public void CancelDash()
    {
        if (_isDashing)
        {
            EndDash();
        }
    }

    public void ResetCooldown()
    {
        _dashCooldownTimer = 0f;
    }
}
