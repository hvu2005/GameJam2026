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
        Debug.Log($"[GravityForm] Gravity {state}! Cooldown: {gravityConfig.gravityCooldown}s");
    }

    private void UpdateCooldown()
    {
        if (skillCooldownTimer > 0f)
        {
            skillCooldownTimer -= Time.deltaTime;
        }
    }
}
