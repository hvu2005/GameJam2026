using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : EventTarget
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerEntity>(out var target))
        {
            OnActivate(target);
        }
    }

    protected virtual void OnActivate(PlayerEntity target)
    {
        
    }
}
