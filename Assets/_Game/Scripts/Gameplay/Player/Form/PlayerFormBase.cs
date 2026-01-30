using UnityEngine;

public abstract class PlayerFormBase : IPlayerForm
{
    protected Player player;
    protected BaseFormConfigSO config;

    public int FormID => config.formID;
    public string FormName => config.formName;
    public abstract bool HasActiveSkill { get; }

    public PlayerFormBase(BaseFormConfigSO config)
    {
        this.config = config;
    }

    public virtual int GetMaxJumps() => config.GetMaxJumps();

    public virtual void OnEnter(Player player)
    {
        this.player = player;
    }

    public virtual void OnUpdate() { }

    public virtual void OnExit() { }

    public virtual void OnSkillPressed() { }
}
