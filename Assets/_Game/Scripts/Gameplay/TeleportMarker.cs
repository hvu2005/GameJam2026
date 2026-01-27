using UnityEngine;
using System;

public class TeleportMarker : MonoBehaviour
{
    public enum MarkerState
    {
        Throwing,
        Flying,
        Landed,
        Expired
    }

    [Header("References")]
    [SerializeField] private PlayerConfig config;
    
    private Rigidbody2D _rb;
    private CircleCollider2D _physicsCollider;
    private CircleCollider2D _pickupTrigger;
    
    private MarkerState _state = MarkerState.Throwing;
    private Vector2 _throwDirection;
    private float _distanceTraveled;
    private float _currentSpeed;
    private float _lifetimeTimer;
    private Transform _playerTransform;
    private float _startTime;
    
    public event Action<TeleportMarker> OnLanded;
    public event Action<TeleportMarker> OnPickedUp;
    public event Action<TeleportMarker> OnExpired;
    
    public MarkerState State => _state;
    public Vector2 Position => transform.position;
    
    public void Initialize(PlayerConfig cfg, Vector2 direction, Transform player)
    {
        config = cfg;
        _throwDirection = direction.normalized;
        _playerTransform = player;
        _startTime = Time.time;
        
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
        }
        
        _physicsCollider = GetComponent<CircleCollider2D>();
        if (_physicsCollider == null)
        {
            _physicsCollider = gameObject.AddComponent<CircleCollider2D>();
            _physicsCollider.radius = 0.2f;
        }
        
        _pickupTrigger = gameObject.AddComponent<CircleCollider2D>();
        _pickupTrigger.radius = config.MarkerPickupRadius;
        _pickupTrigger.isTrigger = true;
        
        Collider2D playerCollider = _playerTransform.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(_physicsCollider, playerCollider);
        }
        
        StartFlight();
    }
    
    void Update()
    {
        switch (_state)
        {
            case MarkerState.Flying:
                UpdateFlight();
                break;
            case MarkerState.Landed:
                UpdateLifetime();
                break;
        }
    }
    
    void FixedUpdate()
    {
        if (_state == MarkerState.Landed)
        {
            return;
        }
    }
    
    private void StartFlight()
    {
        _state = MarkerState.Flying;
        _distanceTraveled = 0f;
        
        if (config.MarkerFlightForce == -1f)
        {
            _currentSpeed = config.MarkerThrowSpeed;
            _rb.velocity = _throwDirection * _currentSpeed;
            _rb.gravityScale = 0f;
        }
        else
        {
            float initialSpeed = config.MarkerFlightForce * config.MarkerThrowSpeed;
            _rb.velocity = _throwDirection * initialSpeed;
            _currentSpeed = initialSpeed;
            _rb.gravityScale = 1f;
        }
    }
    
    private void UpdateFlight()
    {
        if (config.MarkerFlightForce == -1f)
        {
            _distanceTraveled += _rb.velocity.magnitude * Time.deltaTime;
            
            if (_distanceTraveled < config.MarkerMaxDistance)
            {
                float progress = _distanceTraveled / config.MarkerMaxDistance;
                float targetSpeed = CalculateEasedSpeed(progress);
                
                _currentSpeed = Mathf.MoveTowards(
                    _currentSpeed, 
                    targetSpeed, 
                    config.MarkerAcceleration * Time.deltaTime
                );
                
                _rb.velocity = _throwDirection * _currentSpeed;
            }
            else
            {
                _rb.gravityScale = 1f;
            }
        }
        else
        {
            float progress = (Time.time - _startTime) / 2f;
            progress = Mathf.Clamp01(progress);
            float targetSpeed = CalculateEasedSpeed(progress);
            
            Vector2 currentDir = _rb.velocity.normalized;
            if (currentDir.magnitude > 0.1f)
            {
                _rb.velocity = currentDir * targetSpeed;
            }
        }
    }
    
    private float CalculateEasedSpeed(float t)
    {
        float eased;
        if (t < 0.5f)
        {
            eased = 2f * t * t;
        }
        else
        {
            float t2 = (t - 0.5f) * 2f;
            eased = 1f - 2f * (1f - t2) * (1f - t2);
        }
        
        float minSpeed = config.MarkerThrowSpeed;
        float maxSpeed = config.MarkerMaxSpeed;
        
        return Mathf.Lerp(minSpeed, maxSpeed, eased);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state != MarkerState.Flying) return;
        
        _state = MarkerState.Landed;
        _lifetimeTimer = config.MarkerLifetime;
        
        _rb.gravityScale = 1f;
        
        OnLanded?.Invoke(this);
    }
    
    private void UpdateLifetime()
    {
        _lifetimeTimer -= Time.deltaTime;
        
        if (_lifetimeTimer <= 0f)
        {
            _state = MarkerState.Expired;
            OnExpired?.Invoke(this);
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_state != MarkerState.Landed) return;
        
        if (other.transform == _playerTransform)
        {
            OnPickedUp?.Invoke(this);
            Destroy(gameObject);
        }
    }
    
    void OnDrawGizmos()
    {
        if (!Application.isPlaying || config == null) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, config.MarkerPickupRadius);
        
        Gizmos.color = _state == MarkerState.Flying ? Color.yellow : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
