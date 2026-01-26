
using UnityEngine;
public abstract class Hazard : EventTarget
{

    [Header("Hazard Config")]
    [SerializeField] protected HazardType type;
    [SerializeField] protected bool isInstantDeath = false;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float knockbackForce = 5f;

    [Header("VFX")]
    [SerializeField] protected ParticleSystem hitVFX;

    [SerializeField] protected Collider2D damageCollider;

    protected Collider2D hazardCollider;

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
        } else {
            target.TakeDamage(damage);
            target.Knockback(transform.position, knockbackForce);
        }

        // 2. Visual/Audio Feedback
        if (hitVFX != null) hitVFX.Play();
        
        // Ví dụ: Play sound qua AudioController (Singleton)
        // SingleBehaviour.Of<AudioController>().PlaySfx("HazardHit");
    }
}