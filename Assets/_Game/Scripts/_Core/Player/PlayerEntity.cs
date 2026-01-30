using UnityEngine;

public abstract class PlayerEntity : EventTarget
{
    public virtual void Die()
    {
        Debug.Log("Player has died.");
    }
}