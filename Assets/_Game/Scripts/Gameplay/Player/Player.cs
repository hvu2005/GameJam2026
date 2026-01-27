
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerJump), typeof(PlayerDash))]
public class Player : PlayerEntity
{
    [Header("Player Configuration")]
    [SerializeField] private PlayerConfig config;
    
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    
    public PlayerMovement Movement => _movement;
    public PlayerJump Jump => _jump;
    public PlayerDash Dash => _dash;
    public PlayerConfig Config => config;
    
    public bool IsGrounded => _movement != null && _movement.IsGrounded;
    public int FacingDirection => _movement != null ? _movement.FacingDirection : 1;

    void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig is not assigned to Player!", this);
        }
    }

    public override void Die()
    {
        Debug.Log("Player has died.");
        
        ResetPlayerState();
        
        // todo: play death animation, emit death event, etc.
    }

    private void ResetPlayerState()
    {
        if (_jump != null) _jump.CancelJump();
        if (_dash != null) _dash.CancelDash();
        
        if (_dash != null) _dash.ResetCooldown();
    }

    public void SetConfig(PlayerConfig newConfig)
    {
        config = newConfig;
    }
}
