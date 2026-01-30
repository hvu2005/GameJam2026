using UnityEngine;

public class GravityForm : PlayerFormBase
{
    private GravityFormConfigSO gravityConfig;
    private PlayerGravityController gravityController;
    private float skillCooldownTimer;

    public override bool HasActiveSkill => true;

    public GravityForm(GravityFormConfigSO config) : base(config)
    {
        gravityConfig = config;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        gravityController = player.GetComponent<PlayerGravityController>();
        skillCooldownTimer = 0f;
        
        if (gravityController != null)
        {
            gravityController.SetReverseGravityConfig(
                gravityConfig.reverseGravityMultiplier, 
                gravityConfig.reverseGravityBoostDuration
            );
        }
        
        Debug.Log("[GravityForm] Entered - Gravity skill ready");
    }

    public override void OnUpdate()
    {
        UpdateCooldown();
    }

    public override void OnExit()
    {
        if (gravityController != null && gravityController.IsGravityFlipped)
        {
            gravityController.ToggleGravity();
            Debug.Log("[GravityForm] Exited - Gravity reset");
        }
    }

    public override void OnSkillPressed()
    {
        if (skillCooldownTimer > 0f)
        {
            Debug.Log($"[GravityForm] Skill on cooldown: {skillCooldownTimer:F1}s");
            return;
        }

        if (gravityController == null) return;

        gravityController.ToggleGravity();
        skillCooldownTimer = gravityConfig.gravityCooldown;
        
        string state = gravityController.IsGravityFlipped ? "flipped" : "reset";
        
        EventBus.Emit(FormEventType.OnFormSkillActivated,
            new SkillActivatedData
            {
                FormID = FormID,
                SkillName = "GravityFlip"
            });
        Debug.Log($"[GravityForm] [Emit Event] Skill Activated: GravityFlip (Gravity {state})");
        
        EventBus.Emit(FormEventType.OnFormSkillCooldownStart, gravityConfig.gravityCooldown);
        Debug.Log($"[GravityForm] [Emit Event] Skill Cooldown Started: {gravityConfig.gravityCooldown}s");
    }

    private void UpdateCooldown()
    {
        if (skillCooldownTimer > 0f)
        {
            skillCooldownTimer -= Time.deltaTime;
        }
    }
}
