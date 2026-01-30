using System.Collections.Generic;
using UnityEngine;

public abstract class Hazard : EventTarget
{

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hazard triggered by: " + collision.name);
        if (collision.TryGetComponent<PlayerEntity>(out var target))
        {
            OnActivate(target);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerEntity>(out var target))
        {
            OnActivate(target);
        }
    }

    protected virtual void OnActivate(PlayerEntity target)
    {

    }
}
