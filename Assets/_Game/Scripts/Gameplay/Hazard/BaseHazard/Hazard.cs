
using UnityEngine;
public abstract class Hazard : EventTarget
{

    [Header("Hazard Config")]
    [SerializeField] protected bool isInstantDeath = false;

    [SerializeField] protected Collider2D damageCollider;

    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider2D>();
        if (damageCollider == null)
        {
            Debug.LogError("Hazard requires a Collider2D component.");
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IAffectable>(out var target))
        {
            ApplyEffect(target);
        }
    }

    protected virtual void ApplyEffect(IAffectable target)
    {
        if (isInstantDeath) {
            target.Die();
        }
    }
}