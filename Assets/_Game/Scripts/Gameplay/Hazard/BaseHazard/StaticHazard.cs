using System;
using UnityEngine;

public class StaticHazard : Hazard
{
    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);
        //Player die ngay khi chạm vào hazard tĩnh
        target.Die();
    }
}