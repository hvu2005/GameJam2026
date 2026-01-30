using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaLake : StaticHazard
{
    [SerializeField] private ParticleSystem hitVFX;

    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);
        hitVFX?.Play();
    }
}
