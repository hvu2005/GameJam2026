using UnityEngine;

public class VoidForm : PlayerFormBase
{
    private VoidFormConfigSO voidConfig;
    private PlayerTeleportMarker teleportMarker;
    private PlayerTeleportTrail teleportTrail;

    public override bool HasActiveSkill => true;

    public VoidForm(VoidFormConfigSO config) : base(config)
    {
        voidConfig = config;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        
        teleportMarker = player.GetComponent<PlayerTeleportMarker>();
        if (teleportMarker != null)
        {
            teleportMarker.SetVoidConfig(voidConfig);
            teleportMarker.enabled = true;
        }
        
        teleportTrail = player.GetComponent<PlayerTeleportTrail>();
        if (teleportTrail != null)
        {
            teleportTrail.enabled = true;
        }
        
        Debug.Log("[VoidForm] Entered - Teleport enabled");
    }

    public override void OnExit()
    {
        if (teleportMarker != null)
        {
            teleportMarker.enabled = false;
        }
        
        if (teleportTrail != null)
        {
            teleportTrail.ClearTrail();
            teleportTrail.enabled = false;
        }
        
        Debug.Log("[VoidForm] Exited - Teleport disabled");
    }

    public override void OnSkillPressed()
    {
        Debug.Log("[VoidForm] Skill pressed - Teleport");
    }
}

