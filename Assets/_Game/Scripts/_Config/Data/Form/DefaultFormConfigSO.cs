using UnityEngine;

[CreateAssetMenu(fileName = "DefaultFormConfig", menuName = "Game/Forms/Default Form", order = 0)]
public class DefaultFormConfigSO : BaseFormConfigSO
{
    public override bool HasActiveSkill => false;
    public override int GetMaxJumps() => 1;
}
