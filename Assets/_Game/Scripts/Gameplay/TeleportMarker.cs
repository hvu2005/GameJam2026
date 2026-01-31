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

    private float _markerThrowSpeed;
    private float _markerThrowAngle;
    private float _markerFlightForce;
    private float _markerMaxDistance;
    private float _markerAcceleration;
    private float _markerMaxSpeed;
    private float _markerLifetime;
    private float _markerPickupRadius;
    
    private Rigidbody2D _rb;
    private Collider2D _physicsCollider;
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
    public Vector2 LandedNormal { get; private set; }
    public Vector2 ArrivalDirection { get; private set; }
    
    public void Initialize(PlayerConfig cfg, Vector2 direction, Transform player)
    {
        _markerThrowSpeed = cfg.MarkerThrowSpeed;
        _markerThrowAngle = cfg.MarkerThrowAngle;
        _markerFlightForce = cfg.MarkerFlightForce;
        _markerMaxDistance = cfg.MarkerMaxDistance;
        _markerAcceleration = cfg.MarkerAcceleration;
        _markerMaxSpeed = cfg.MarkerMaxSpeed;
        _markerLifetime = cfg.MarkerLifetime;
        _markerPickupRadius = cfg.MarkerPickupRadius;
        
        InitializeInternal(direction, player);
    }

    public void InitializeWithVoidConfig(VoidFormConfigSO cfg, Vector2 direction, Transform player)
    {
        _markerThrowSpeed = cfg.markerThrowSpeed;
        _markerThrowAngle = cfg.markerThrowAngle;
        _markerFlightForce = cfg.markerFlightForce;
        _markerMaxDistance = cfg.markerMaxDistance;
        _markerAcceleration = cfg.markerAcceleration;
        _markerMaxSpeed = cfg.markerMaxSpeed;
        _markerLifetime = cfg.markerLifetime;
        _markerPickupRadius = cfg.markerPickupRadius;
        
        InitializeInternal(direction, player);
    }

    private void InitializeInternal(Vector2 direction, Transform player)
    {
        _throwDirection = direction.normalized;
        _playerTransform = player;
        _startTime = Time.time;
        
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
        }
        
        _physicsCollider = GetComponent<Collider2D>();
        if (_physicsCollider == null)
        {
            var circle = gameObject.AddComponent<CircleCollider2D>();
            circle.radius = 0.2f;
            _physicsCollider = circle;
        }
        
        _pickupTrigger = gameObject.AddComponent<CircleCollider2D>();
        _pickupTrigger.radius = _markerPickupRadius;
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
    
    private Vector2 _lastVelocity;

    void FixedUpdate()
    {
        if (_state == MarkerState.Flying)
        {
            _lastVelocity = _rb.velocity;
        }
        
        if (_state == MarkerState.Landed)
        {
            return;
        }
    }
    
    private void StartFlight()
    {
        _state = MarkerState.Flying;
        _distanceTraveled = 0f;
        
        if (_markerFlightForce == -1f)
        {
            _currentSpeed = _markerThrowSpeed;
            _rb.velocity = _throwDirection * _currentSpeed;
            _rb.gravityScale = 0f;
        }
        else
        {
            float initialSpeed = _markerFlightForce * _markerThrowSpeed;
            _rb.velocity = _throwDirection * initialSpeed;
            _currentSpeed = initialSpeed;
            _rb.gravityScale = 1f;
        }
    }
    
    private void UpdateFlight()
    {
        if (Time.time - _startTime > _markerLifetime)
        {
            _state = MarkerState.Expired;
            OnExpired?.Invoke(this);
            Destroy(gameObject);
            return;
        }

        if (_markerFlightForce == -1f)
        {
            _distanceTraveled += _rb.velocity.magnitude * Time.deltaTime;
            
            if (_distanceTraveled < _markerMaxDistance)
            {
                float progress = _distanceTraveled / _markerMaxDistance;
                float targetSpeed = CalculateEasedSpeed(progress);
                
                _currentSpeed = Mathf.MoveTowards(
                    _currentSpeed, 
                    targetSpeed, 
                    _markerAcceleration * Time.deltaTime
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
        
        float minSpeed = _markerThrowSpeed;
        float maxSpeed = _markerMaxSpeed;
        
        return Mathf.Lerp(minSpeed, maxSpeed, eased);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_state != MarkerState.Flying) return;
        
        _state = MarkerState.Landed;
        _lifetimeTimer = _markerLifetime;
        _rb.gravityScale = 1f;
        
        if (collision.contacts.Length > 0)
        {
            LandedNormal = collision.contacts[0].normal;
            
            if (_lastVelocity.sqrMagnitude > 0.01f)
            {
                ArrivalDirection = _lastVelocity.normalized;
            }
            else
            {
                ArrivalDirection = collision.relativeVelocity.normalized * -1f;
            }
        }
        
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
        if (!Application.isPlaying) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _markerPickupRadius);
        
        Gizmos.color = _state == MarkerState.Flying ? Color.yellow : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}

