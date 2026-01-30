using UnityEngine;

/// <summary>
/// Quản lý tất cả các status effects ảnh hưởng đến player
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerStatusEffects : MonoBehaviour
{
    // Movement modifiers
    private float _moveSpeedMultiplier = 1f;
    
    // Jump modifiers
    private float _jumpForceMultiplier = 1f;
    
    
    private bool _isInMud;
    
    private bool _isOnIce;
    private PlayerConfig _config;
    
    public float MoveSpeedMultiplier => _moveSpeedMultiplier;
    public float JumpForceMultiplier => _jumpForceMultiplier;
    public bool IsInMud => _isInMud;
    
    public bool IsOnIce 
    { 
        get => _isOnIce; 
        set => _isOnIce = value;
    }

    void Awake()
    {
        _config = GetComponent<Player>().Config;
    }

    void OnEnable()
    {
        // Subscribe to environment events
        EventBus.On<MudTile>(EnvironmentEventType.EnterMud, OnEnterMud);
        EventBus.On<MudTile>(EnvironmentEventType.ExitMud, OnExitMud);
    }

    void OnDisable()
    {
        // Unsubscribe from events
        EventBus.Off<MudTile>(EnvironmentEventType.EnterMud, OnEnterMud);
        EventBus.Off<MudTile>(EnvironmentEventType.ExitMud, OnExitMud);
    }

    #region Mud Effects
    
    private void OnEnterMud(MudTile mud)
    {
        _isInMud = true;
        _moveSpeedMultiplier = 1f - mud.MoveSpeedReduction;
        _jumpForceMultiplier = 1f - mud.JumpForceReduction;
    }

    private void OnExitMud(MudTile mud)
    {
        _isInMud = false;
        ResetModifiers();
    }
    
    #endregion

    /// <summary>
    /// Reset tất cả modifiers về giá trị mặc định
    /// </summary>
    public void ResetModifiers()
    {
        _moveSpeedMultiplier = 1f;
        _jumpForceMultiplier = 1f;
    }

    /// <summary>
    /// Áp dụng một multiplier tùy chỉnh (cho power-ups, buffs, etc.)
    /// </summary>
    public void ApplyCustomModifier(float moveMultiplier, float jumpMultiplier)
    {
        _moveSpeedMultiplier *= moveMultiplier;
        _jumpForceMultiplier *= jumpMultiplier;
    }
    
    #region Ice Effects
    
    public float GetCurrentAccelerationMultiplier()
    {
        if (_config == null) return 1f;
        
        float multiplier = 1f;
        
        if (_isOnIce)
        {
            multiplier *= _config.IceAccelerationMultiplier;
        }
        
        multiplier *= _moveSpeedMultiplier;
        
        return multiplier;
    }

    public float GetCurrentDecelerationMultiplier(float currentSpeed, float maxSpeed)
    {
        if (_config == null) return 1f;
        
        float multiplier = 1f;
        
        if (_isOnIce)
        {
            multiplier *= _config.IceDecelerationMultiplier;
            
            if (_config.UseVelocityBasedMomentum && maxSpeed > 0f)
            {
                float speedRatio = Mathf.Abs(currentSpeed) / maxSpeed;
                float curveValue = _config.IceDecelerationCurve.Evaluate(speedRatio);
                multiplier *= curveValue;
            }
        }
        
        return multiplier;
    }
    
    #endregion
}
