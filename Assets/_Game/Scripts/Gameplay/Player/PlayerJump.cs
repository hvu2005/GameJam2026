
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

    private int _jumpCount;
    private float _lastGroundedTime = -999f;
    private float _lastJumpTime = -999f;

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
    }

    private void OnJumpInput(bool pressed)
    {
        if (!pressed) return;
        
        // Cancel dash if currently dashing
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

        bool withinCoyoteTime = Time.time - _lastGroundedTime < config.CoyoteTime;
        bool hasGroundJump = (_movement.IsGrounded || withinCoyoteTime) && _jumpCount == 0;

        if (hasGroundJump)
        {
            return true;
        }
        
        if (_jumpCount > 0 && _jumpCount < config.MaxJumps)
        {
            return true;
        }

        return false;
    }

    private void PerformJump()
    {
        if (config == null) return;

        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        
        float jumpForce = _jumpCount == 0 ? config.JumpForce : config.DoubleJumpForce;
        // Áp dụng status effects multiplier vào lực nhảy
        float multiplier = _statusEffects != null ? _statusEffects.JumpForceMultiplier : 1f;
        jumpForce *= multiplier;
        _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);

        _jumpCount++;
        _isJumping = true;
        _lastJumpTime = Time.time;
    }

    private void ApplyGravityModifiers()
    {
        if (config == null) return;

        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (config.FallGravityMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnLanded()
    {
        _isJumping = false;
    }

    public void CancelJump()
    {
        if (_rb.velocity.y > 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        }
        _isJumping = false;
    }
}
