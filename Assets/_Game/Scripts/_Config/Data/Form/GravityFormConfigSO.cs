using UnityEngine;

[CreateAssetMenu(fileName = "GravityFormConfig", menuName = "Game/Forms/Gravity Form", order = 3)]
public class GravityFormConfigSO : BaseFormConfigSO
{
    [Header("Gravity Settings")]
    [Tooltip("Duration of gravity flip effect")]
    public float gravityFlipDuration = 2f;
    
    [Tooltip("Gravity scale when flipped (-1 = inverted)")]
    public float flippedGravityScale = -1f;
    
    [Tooltip("Hệ số nhân gravity tạm thời khi đảo (>1 = bay/rơi nhanh hơn)")]
    public float reverseGravityMultiplier = 2f;
    
    [Tooltip("Thời gian tăng gravity tạm thời (giây)")]
    public float reverseGravityBoostDuration = 0.3f;
    
    [Tooltip("Cooldown for gravity skill")]
    public float gravityCooldown = 0.5f;

    public override bool HasActiveSkill => true;
    public override int GetMaxJumps() => 1;
}
