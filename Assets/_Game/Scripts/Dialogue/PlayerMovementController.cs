using UnityEngine;

public static class PlayerMovementController
{
    public static void Disable(GameObject player)
    {
        if (player == null) return;

        // CRITICAL ORDER: Disable components FIRST, then stop physics
        // This prevents FixedUpdate from running and re-applying velocity

        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Now safe to stop physics (no FixedUpdate will run)
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public static void Enable(GameObject player)
    {
        if (player == null) return;

        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}
