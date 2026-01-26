
using UnityEngine;

public class Player : Entity, IAffectable
{
    public void Die()
    {
        Debug.Log("Player has died.");
    }

    public void Knockback(Vector2 sourcePosition, float force)
    {
        Debug.Log("Player knocked back.");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Player took {damage} damage.");
    }
}