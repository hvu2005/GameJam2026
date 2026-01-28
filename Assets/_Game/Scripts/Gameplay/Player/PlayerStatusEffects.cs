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
    
    // State tracking
    private bool _isInMud;
    
    public float MoveSpeedMultiplier => _moveSpeedMultiplier;
    public float JumpForceMultiplier => _jumpForceMultiplier;
    public bool IsInMud => _isInMud;

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
}
