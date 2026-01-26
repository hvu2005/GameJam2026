using System;
using UnityEngine;

public class StaticHazard : Hazard {
    // Không cần logic thêm vì Hazard base class đã xử lý va chạm và instant death
    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitVFX;
    protected virtual void Reset() {
        isInstantDeath = true; 
    }
    protected override void ApplyEffect(IAffectable target)
    {
        base.ApplyEffect(target);
        hitVFX?.Play();
        //TODO: Sound effect
    }
}