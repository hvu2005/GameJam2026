using UnityEngine;

[CreateAssetMenu(fileName = "GravityFormConfig", menuName = "Game/Forms/Gravity Form", order = 3)]
public class GravityFormConfigSO : BaseFormConfigSO
{
    [Header("Gravity Settings")]
    [Tooltip("Duration of gravity flip effect")]
    public float gravityFlipDuration = 2f;
    
    [Tooltip("Gravity scale when flipped (-1 = inverted)")]
    public float flippedGravityScale = -1f;
    
    [Tooltip("Cooldown for gravity skill")]
    public float gravityCooldown = 0.5f;

    public override bool HasActiveSkill => true;
    public override int GetMaxJumps() => 1;
}
