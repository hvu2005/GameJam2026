using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerTeleportMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject markerPrefab;
    
    private PlayerInput _input;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    private Rigidbody2D _rb;
    
    private TeleportMarker _activeMarker;
    private float _teleportWindowTimer;
    private float _cooldownTimer;
    private bool _canTeleport;
    
    public bool CanUseSkill => _cooldownTimer <= 0f && _activeMarker == null;
    public bool CanTeleport => _canTeleport && _activeMarker != null;
    public float CooldownPercent => Mathf.Clamp01(1f - (_cooldownTimer / config.TeleportCooldown));
    
    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        _rb = GetComponent<Rigidbody2D>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig not assigned to PlayerTeleportMarker!", this);
        }
        
        if (markerPrefab == null)
        {
            Debug.LogError("Marker prefab not assigned to PlayerTeleportMarker!", this);
        }
    }
    
    void OnEnable()
    {
        _input.On<bool>(PlayerInputType.Skill, OnSkillInput);
    }
    
    void OnDisable()
    {
        _input.Off<bool>(PlayerInputType.Skill, OnSkillInput);
    }
    
    void Update()
    {
        UpdateCooldown();
        UpdateTeleportWindow();
    }
    
    private void OnSkillInput(bool pressed)
    {
        if (!pressed) return;
        
        if (_activeMarker == null && CanUseSkill)
        {
            ThrowMarker();
            _canTeleport = true;
            _teleportWindowTimer = config.TeleportWindowTime;
        }
        else if (CanTeleport)
        {
            TeleportToMarker();
        }
    }
    
    private void ThrowMarker()
    {
        if (config == null || markerPrefab == null) return;
        
        Vector2 direction = CalculateThrowDirection();
        
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject markerObj = Instantiate(markerPrefab, spawnPos, Quaternion.identity);
        
        _activeMarker = markerObj.GetComponent<TeleportMarker>();
        if (_activeMarker != null)
        {
            _activeMarker.Initialize(config, direction, transform);
            
            _activeMarker.OnLanded += OnMarkerLanded;
            _activeMarker.OnPickedUp += OnMarkerPickedUp;
            _activeMarker.OnExpired += OnMarkerExpired;
        }
        else
        {
            Debug.LogError("Marker prefab missing TeleportMarker component!");
            Destroy(markerObj);
        }
    }
    
    private Vector2 CalculateThrowDirection()
    {
        Vector2 inputDirection = _input.MoveInput;
        
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            return inputDirection.normalized;
        }
        
        float facingAngle = _movement.FacingDirection > 0 ? 0f : 180f;
        float totalAngle = facingAngle + config.MarkerThrowAngle;
        
        float radians = totalAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }
    
    private void OnMarkerLanded(TeleportMarker marker)
    {
        _canTeleport = true;
        _teleportWindowTimer = config.TeleportWindowTime;
    }
    
    private void OnMarkerPickedUp(TeleportMarker marker)
    {
        _cooldownTimer = 0f;
        _activeMarker = null;
        _canTeleport = false;
    }
    
    private void OnMarkerExpired(TeleportMarker marker)
    {
        _activeMarker = null;
        _canTeleport = false;
    }
    
    private void TeleportToMarker()
    {
        if (_activeMarker == null) return;
        
        Vector2 targetPos = _activeMarker.Position;
        Vector2 validPos = FindValidTeleportPosition(targetPos);
        
        _rb.velocity = Vector2.zero;
        
        if (_dash != null && _dash.IsDashing)
        {
            _dash.CancelDash();
        }
        
        transform.position = validPos;
        
        Destroy(_activeMarker.gameObject);
        _activeMarker = null;
        _canTeleport = false;
        
        _cooldownTimer = config.TeleportCooldown;
    }
    
    private Vector2 FindValidTeleportPosition(Vector2 targetPos)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null)
        {
            return targetPos;
        }
        
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPos - currentPos).normalized;
        float distance = Vector2.Distance(currentPos, targetPos);
        
        float playerRadius = Mathf.Max(
            playerCollider.bounds.extents.x,
            playerCollider.bounds.extents.y
        );
        
        // Strategy 1: Raycast from player to marker to find obstruction
        RaycastHit2D hit = Physics2D.CircleCast(
            currentPos,
            playerRadius * 0.9f,
            direction,
            distance,
            config.GroundLayer
        );
        
        if (hit.collider != null)
        {
            // Hit obstruction - teleport just before it
            Vector2 positionBeforeWall = hit.point - direction * (playerRadius + 0.1f);
            
            if (!IsPositionBlocked(positionBeforeWall, playerCollider))
            {
                return positionBeforeWall;
            }
        }
        
        // Strategy 2: Try exact marker position
        if (!IsPositionBlocked(targetPos, playerCollider))
        {
            return targetPos;
        }
        
        // Strategy 3: Try positions around marker (spiral search)
        Vector2[] offsets = new Vector2[]
        {
            Vector2.up * config.TeleportOffsetY,
            Vector2.right * config.TeleportOffsetY,
            Vector2.left * config.TeleportOffsetY,
            Vector2.down * config.TeleportOffsetY * 0.5f,
            new Vector2(1, 1).normalized * config.TeleportOffsetY,
            new Vector2(-1, 1).normalized * config.TeleportOffsetY,
            new Vector2(1, -1).normalized * config.TeleportOffsetY,
            new Vector2(-1, -1).normalized * config.TeleportOffsetY
        };
        
        for (int mult = 1; mult <= config.TeleportMaxAttempts; mult++)
        {
            foreach (Vector2 offset in offsets)
            {
                Vector2 testPos = targetPos + offset * mult;
                
                if (!IsPositionBlocked(testPos, playerCollider))
                {
                    return testPos;
                }
            }
        }
        
        // Strategy 4: Fallback - stay at current position
        Debug.LogWarning("Cannot find valid teleport position, staying in place!");
        return currentPos;
    }
    
    private bool IsPositionBlocked(Vector2 pos, Collider2D playerCollider)
    {
        float checkRadius = Mathf.Max(
            playerCollider.bounds.extents.x, 
            playerCollider.bounds.extents.y
        );
        
        Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, checkRadius * 0.9f);
        
        foreach (var overlap in overlaps)
        {
            if (overlap != playerCollider && !overlap.isTrigger)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void UpdateTeleportWindow()
    {
        if (!_canTeleport) return;
        
        _teleportWindowTimer -= Time.deltaTime;
        
        if (_teleportWindowTimer <= 0f)
        {
            _canTeleport = false;
        }
    }
    
    private void UpdateCooldown()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }
    
    void OnDrawGizmos()
    {
        if (firePoint != null && config != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
            
            if (Application.isPlaying && _movement != null)
            {
                Vector2 dir = CalculateThrowDirection();
                Gizmos.color = CanUseSkill ? Color.green : Color.red;
                Gizmos.DrawRay(firePoint.position, dir * 2f);
            }
        }
    }
}
