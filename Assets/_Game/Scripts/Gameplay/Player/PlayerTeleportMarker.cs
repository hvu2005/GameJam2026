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
    private float _teleportWindowEndTime;
    private float _cooldownEndTime;
    private bool _canTeleport;
    
    public bool CanUseSkill => _config != null && Time.time >= _cooldownEndTime && _activeMarker == null;
    public bool CanTeleport => _canTeleport && _activeMarker != null && Time.time < _teleportWindowEndTime;
    public float CooldownPercent => _config != null ? Mathf.Clamp01((Time.time - (_cooldownEndTime - _config.teleportCooldown)) / _config.teleportCooldown) : 1f;
    public float CooldownRemaining => Mathf.Max(0f, _cooldownEndTime - Time.time);
    public float TotalCooldownDuration => _config != null ? _config.teleportCooldown : 0f;
    
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
            _teleportWindowEndTime = Time.time + _config.teleportWindowTime;
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
        
        // Only use horizontal input (left/right), ignore vertical
        if (Mathf.Abs(inputDirection.x) > 0.01f)
        {
            // Return pure horizontal direction based on X input
            return new Vector2(Mathf.Sign(inputDirection.x), 0f);
        }
        
        // Fallback to facing direction (pure horizontal)
        float facingDirection = _movement.FacingDirection > 0 ? 1f : -1f;
        return new Vector2(facingDirection, 0f);
    }
    
    private void OnMarkerLanded(TeleportMarker marker)
    {
        _canTeleport = true;
        _teleportWindowEndTime = Time.time + _config.teleportWindowTime;
    }
    
    private void OnMarkerPickedUp(TeleportMarker marker)
    {
        _cooldownEndTime = 0f;
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
        Vector2? validPos = FindValidTeleportPosition(targetPos);
        
        if (validPos == null)
        {
            return;
        }

        _rb.velocity = Vector2.zero;
        
        if (_dash != null && _dash.IsDashing)
        {
            _dash.CancelDash();
        }
        
        Vector3 oldPosition = transform.position;
        
        transform.position = validPos.Value;
        
        PlayerTeleportTrail trail = GetComponent<PlayerTeleportTrail>();
        if (trail != null)
        {
            trail.ActivateTrail(oldPosition, validPos.Value);
        }
        
        EventBus.Emit(FormEventType.OnFormSkillCooldownStart, _config.teleportCooldown);
        Debug.Log($"[PlayerTeleportMarker] [Emit Event] Skill Cooldown Started: {_config.teleportCooldown}s");
        
        Destroy(_activeMarker.gameObject);
        _activeMarker = null;
        _canTeleport = false;
        
        _cooldownEndTime = Time.time + _config.teleportCooldown;
    }
    
    private Vector2? FindValidTeleportPosition(Vector2 targetPos)
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider == null) return targetPos; 

        Vector2 size = playerCollider.bounds.size;
        Vector2 checkSize = size * 0.95f; 
        
        bool CheckOverlap(Vector2 pos)
        {
             Collider2D hit = Physics2D.OverlapBox(pos + playerCollider.offset, checkSize, 0f, _config.groundLayer);
             return hit != null;
        }

        if (!CheckOverlap(targetPos))
        {
            return ApplyGroundSnap(targetPos, size);
        }

        Vector2 normal = _activeMarker != null ? _activeMarker.LandedNormal : Vector2.zero;
        if (normal != Vector2.zero)
        {
            float nudgeDist = Mathf.Abs(Vector2.Dot(normal, Vector2.right)) * size.x * 0.5f 
                            + Mathf.Abs(Vector2.Dot(normal, Vector2.up)) * size.y * 0.5f;
            
            Vector2 testPos = targetPos + normal * (nudgeDist + 0.1f);
            if (!CheckOverlap(testPos))
            {
                return ApplyGroundSnap(testPos, size);
            }
        }

        Vector2 testPosUp = targetPos + Vector2.up * size.y;
        if (!CheckOverlap(testPosUp))
        {
            return ApplyGroundSnap(testPosUp, size);
        }

        Vector2 arrivalDir = _activeMarker != null ? _activeMarker.ArrivalDirection : Vector2.zero;
        if (arrivalDir != Vector2.zero)
        {
            Vector2 backtrackDir = -arrivalDir.normalized;
            for(int i=1; i<=20; i++)
            {
                float dist = 0.2f * i;
                Vector2 testPos = targetPos + backtrackDir * dist;
                
                if (!CheckOverlap(testPos))
                {
                    return ApplyGroundSnap(testPos, size);
                }
            }
        }
        
        Vector2[] offsets = new Vector2[]
        {
            Vector2.up * 0.5f,
            Vector2.right * 0.5f,
            Vector2.left * 0.5f,
            new Vector2(1,1).normalized * 0.5f,
            new Vector2(-1,1).normalized * 0.5f
        };

        foreach (var offset in offsets)
        {
            Vector2 testPos = targetPos + offset;
            if (!CheckOverlap(testPos))
            {
               return ApplyGroundSnap(testPos, size);
            }
        }

        return null; 
    }

    private Vector2 ApplyGroundSnap(Vector2 pos, Vector2 playerSize)
    {
        float halfHeight = playerSize.y * 0.5f;
        Vector2 bottomPos = pos - new Vector2(0, halfHeight);

        float snapDist = 1.0f;
        RaycastHit2D hit = Physics2D.Raycast(bottomPos, Vector2.down, snapDist, _config.groundLayer);

        if (hit.collider != null)
        {
            return new Vector2(pos.x, hit.point.y + halfHeight + 0.01f);
        }
        return pos;
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
        
        if (Time.time >= _teleportWindowEndTime)
        {
            _canTeleport = false;
        }
    }
    
    private void UpdateCooldown()
    {
        // No longer needed - cooldown is checked via Time.time comparison
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
