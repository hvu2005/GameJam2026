
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Transform visualTransform;
    
    private PlayerInput _input;
    private Rigidbody2D _rb;
    private PlayerDash _dash;
    private PlayerStatusEffects _statusEffects;
    private float _currentSpeed;
    private float _targetSpeed;
    private int _facingDirection = 1;
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _wasOnIce;

    public int FacingDirection => _facingDirection;
    public bool IsGrounded => _isGrounded;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _rb = GetComponent<Rigidbody2D>();
        _dash = GetComponent<PlayerDash>();
        _statusEffects = GetComponent<PlayerStatusEffects>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig is not assigned to PlayerMovement2D!", this);
        }
    }

    void OnEnable()
    {
        _input.On<Vector2>(PlayerInputType.Move, OnMove);
    }

    void OnDisable()
    {
        _input.Off<Vector2>(PlayerInputType.Move, OnMove);
    }

    void FixedUpdate()
    {
        _wasGrounded = _isGrounded;
        
        UpdateGroundDetection();
        
        if (_dash != null && _dash.IsDashing)
        {
            return;
        }
        
        UpdateMovement();
    }

    private void OnMove(Vector2 direction)
    {
        // Áp dụng status effects multiplier vào tốc độ
        float multiplier = _statusEffects != null ? _statusEffects.MoveSpeedMultiplier : 1f;
        _targetSpeed = direction.x * config.MoveSpeed * multiplier;
        
        if (direction.x > 0.1f)
        {
            SetFacingDirection(1);
        }
        else if (direction.x < -0.1f)
        {
            SetFacingDirection(-1);
        }
    }

    private void UpdateMovement()
    {
        if (config == null) return;

        float accelerationRate;
        
        if (Mathf.Abs(_targetSpeed) > 0.01f)
        {
            float accelMultiplier = _statusEffects != null 
                ? _statusEffects.GetCurrentAccelerationMultiplier() 
                : 1f;
            accelerationRate = config.Acceleration * accelMultiplier;
        }
        else
        {
            float decelMultiplier = _statusEffects != null 
                ? _statusEffects.GetCurrentDecelerationMultiplier(_currentSpeed, config.MoveSpeed) 
                : 1f;
            accelerationRate = config.Deceleration * decelMultiplier;
        }

        if (!_isGrounded)
        {
            float airControlMult = config.AirControlMultiplier;
            
            if (_wasOnIce)
            {
                airControlMult *= config.IceAirControlPenalty;
            }
            
            accelerationRate *= airControlMult;
        }

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, _targetSpeed, accelerationRate * Time.fixedDeltaTime);

        _rb.velocity = new Vector2(_currentSpeed, _rb.velocity.y);
    }

    private void UpdateGroundDetection()
    {
        if (config == null) return;

        Vector2 boxOrigin = (Vector2)transform.position + config.GroundCheckOffset;
        Vector2 boxSize = new Vector2(0.8f, 0.8f);
        
        RaycastHit2D hit = Physics2D.BoxCast(
            boxOrigin, 
            boxSize, 
            0f, 
            Vector2.down, 
            config.GroundCheckDistance, 
            config.GroundLayer
        );
        
        _isGrounded = hit.collider != null;

        if (_statusEffects != null)
        {
            if (_isGrounded && hit.collider != null)
            {
                if (hit.collider.CompareTag("Ice"))
                {
                    _statusEffects.IsOnIce = true;
                }
                else
                {
                    _statusEffects.IsOnIce = false;
                }
            }
            else
            {
                _statusEffects.IsOnIce = false;
            }
        }
        
        if (!_wasGrounded && _isGrounded)
        {
            _wasOnIce = false;
        }
        else if (_wasGrounded && !_isGrounded)
        {
            _wasOnIce = _statusEffects != null && _statusEffects.IsOnIce;
        }
    }

    private void SetFacingDirection(int direction)
    {
        if (_facingDirection == direction) return;

        _facingDirection = direction;
        
        if (visualTransform != null)
        {
            Vector3 scale = visualTransform.localScale;
            scale.x = Mathf.Abs(scale.x) * _facingDirection;
            visualTransform.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * _facingDirection;
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (config == null) return;

        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)transform.position + config.GroundCheckOffset;
        Gizmos.DrawLine(origin, origin + Vector2.down * config.GroundCheckDistance);
    }
}
