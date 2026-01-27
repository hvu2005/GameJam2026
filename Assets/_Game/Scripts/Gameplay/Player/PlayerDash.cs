
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput), typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;
    
    private PlayerInput _input;
    private Rigidbody2D _rb;
    private PlayerMovement _movement;
    
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;
    private Vector2 _dashDirection;
    private float _originalGravityScale;
    private bool _hasLandedSinceDash = true;

    public bool IsDashing => _isDashing;
    public bool CanDash => _dashCooldownTimer <= 0f && (!_isDashing) && _hasLandedSinceDash;
    public float DashCooldownPercent => Mathf.Clamp01(1f - (_dashCooldownTimer / config.DashCooldown));

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement>();
        _originalGravityScale = _rb.gravityScale;
        
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
        
        // Track landing after dash
        if (_movement.IsGrounded && !_isDashing)
        {
            _hasLandedSinceDash = true;
        }
    }

    private void OnDashInput(bool pressed)
    {
        if (!pressed) return;
        
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
        
        _dashDirection = new Vector2(_movement.FacingDirection, 0f).normalized;
        
        _rb.gravityScale = 0f;
        _rb.velocity = _dashDirection * config.DashSpeed;
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
        
        _rb.gravityScale = _originalGravityScale;
        
        _rb.velocity = new Vector2(_rb.velocity.x * 0.5f, _rb.velocity.y);
    }

    private void UpdateCooldown()
    {
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
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
