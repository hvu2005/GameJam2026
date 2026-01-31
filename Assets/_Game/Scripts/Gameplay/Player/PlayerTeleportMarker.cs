using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerTeleportMarker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    
    private VoidFormConfigSO _config;
    private PlayerInput _input;
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    private Rigidbody2D _rb;
    
    private TeleportMarker _activeMarker;
    private float _teleportWindowTimer;
    private float _cooldownTimer;
    private bool _canTeleport;
    
    public bool CanUseSkill => _config != null && _cooldownTimer <= 0f && _activeMarker == null;
    public bool CanTeleport => _canTeleport && _activeMarker != null;
    public float CooldownPercent => _config != null ? Mathf.Clamp01(1f - (_cooldownTimer / _config.teleportCooldown)) : 0f;
    
    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        _rb = GetComponent<Rigidbody2D>();
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

    public void SetVoidConfig(VoidFormConfigSO config)
    {
        _config = config;
        Debug.Log($"[PlayerTeleportMarker] Config set: {config.formName}");
    }
    
    private void OnSkillInput(bool pressed)
    {
        if (!pressed) return;
        if (_config == null) return;
        
        if (_activeMarker == null && CanUseSkill)
        {
            Debug.Log("[PlayerTeleportMarker] Throwing marker");
            ThrowMarker();
            _canTeleport = true;
            _teleportWindowTimer = _config.teleportWindowTime;
        }
        else if (CanTeleport)
        {
            Debug.Log("[PlayerTeleportMarker] Teleporting to marker");
            TeleportToMarker();
        }
    }
    
    private void ThrowMarker()
    {
        if (_config == null || _config.markerPrefab == null) return;
        
        Vector2 direction = CalculateThrowDirection();
        
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject markerObj = Instantiate(_config.markerPrefab, spawnPos, Quaternion.identity);
        
        _activeMarker = markerObj.GetComponent<TeleportMarker>();
        if (_activeMarker != null)
        {
            _activeMarker.InitializeWithVoidConfig(_config, direction, transform);
            
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
        float totalAngle = facingAngle + _config.markerThrowAngle;
        
        float radians = totalAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
    }
    
    private void OnMarkerLanded(TeleportMarker marker)
    {
        _canTeleport = true;
        _teleportWindowTimer = _config.teleportWindowTime;
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
        if (_activeMarker == null || _config == null) return;
        
        Vector2 targetPos = _activeMarker.Position;
        Vector2 validPos = FindValidTeleportPosition(targetPos);
        
        _rb.velocity = Vector2.zero;
        
        if (_dash != null && _dash.IsDashing)
        {
            _dash.CancelDash();
        }
        
        // Lưu vị trí cũ để tạo trail
        Vector3 oldPosition = transform.position;
        
        // Dịch chuyển đến vị trí mới
        transform.position = validPos;
        
        // Kích hoạt trail effect
        PlayerTeleportTrail trail = GetComponent<PlayerTeleportTrail>();
        if (trail != null)
        {
            trail.ActivateTrail(oldPosition, validPos);
        }
        
        // Emit event để UI hiển thị cooldown
        EventBus.Emit(FormEventType.OnFormSkillCooldownStart, _config.teleportCooldown);
        Debug.Log($"[PlayerTeleportMarker] [Emit Event] Skill Cooldown Started: {_config.teleportCooldown}s");
        
        Destroy(_activeMarker.gameObject);
        _activeMarker = null;
        _canTeleport = false;
        
        _cooldownTimer = _config.teleportCooldown;
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
        
        RaycastHit2D hit = Physics2D.CircleCast(
            currentPos,
            playerRadius * 0.9f,
            direction,
            distance,
            _config.groundLayer
        );
        
        if (hit.collider != null)
        {
            Vector2 positionBeforeWall = hit.point - direction * (playerRadius + 0.1f);
            
            if (!IsPositionBlocked(positionBeforeWall, playerCollider))
            {
                return positionBeforeWall;
            }
        }
        
        if (!IsPositionBlocked(targetPos, playerCollider))
        {
            return targetPos;
        }
        
        Vector2[] offsets = new Vector2[]
        {
            Vector2.up * _config.teleportOffsetY,
            Vector2.right * _config.teleportOffsetY,
            Vector2.left * _config.teleportOffsetY,
            Vector2.down * _config.teleportOffsetY * 0.5f,
            new Vector2(1, 1).normalized * _config.teleportOffsetY,
            new Vector2(-1, 1).normalized * _config.teleportOffsetY,
            new Vector2(1, -1).normalized * _config.teleportOffsetY,
            new Vector2(-1, -1).normalized * _config.teleportOffsetY
        };
        
        for (int mult = 1; mult <= _config.teleportMaxAttempts; mult++)
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
        if (firePoint != null && _config != null)
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
