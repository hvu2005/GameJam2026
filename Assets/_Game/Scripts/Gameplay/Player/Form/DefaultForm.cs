public class DefaultForm : PlayerFormBase
{
    public override bool HasActiveSkill => false;

    public DefaultForm(DefaultFormConfigSO config) : base(config) { }
}
