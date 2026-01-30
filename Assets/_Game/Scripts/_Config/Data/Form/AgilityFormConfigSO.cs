using UnityEngine;

[CreateAssetMenu(fileName = "AgilityFormConfig", menuName = "Game/Forms/Agility Form", order = 1)]
public class AgilityFormConfigSO : BaseFormConfigSO
{
    [Header("Agility Settings")]
    [Tooltip("Extra jumps granted by this form")]
    public int extraJumpCount = 1;

    public override bool HasActiveSkill => false;
    public override int GetMaxJumps() => 1 + extraJumpCount;
}
