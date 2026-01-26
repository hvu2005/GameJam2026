using UnityEngine;

public interface IAffectable
{
    void TakeDamage(int damage);
    void Die();
    void Knockback(Vector2 sourcePosition, float force);

    Transform transform { get;  }
}