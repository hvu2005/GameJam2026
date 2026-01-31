using UnityEngine;

public static class PlayerMovementController
{
    private static RigidbodyConstraints2D _originalConstraints;

    public static void Disable(GameObject player)
    {
        if (player == null) return;

        // CRITICAL ORDER: Disable components FIRST, then stop physics
        // This prevents FixedUpdate from running and re-applying velocity

        // 1. Change state to Idle to stop all state-based movement
        var stateMachine = player.GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerState.Idle);
        }

        // 2. Disable input (no new input accepted)
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // 3. Disable movement component (no FixedUpdate execution)
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 4. Freeze horizontal movement but ALLOW vertical (gravity/fall)
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Save original constraints
            _originalConstraints = rb.constraints;

            // Stop horizontal movement only
            rb.velocity = new Vector2(0f, rb.velocity.y); // Keep Y velocity for falling
            rb.angularVelocity = 0f;

            // FREEZE X position only - allow Y for gravity/falling
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public static void Enable(GameObject player)
    {
        if (player == null) return;

        // 1. Restore Rigidbody constraints FIRST
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.constraints = _originalConstraints;
        }

        // 2. Re-enable input
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        // 3. Re-enable movement
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}