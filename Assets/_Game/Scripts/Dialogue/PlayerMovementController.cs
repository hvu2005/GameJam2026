using UnityEngine;

public static class PlayerMovementController
{
    private static RigidbodyConstraints2D _originalConstraints;

    public static void Disable(GameObject player)
    {
        if (player == null) return;

        // CRITICAL ORDER: Disable components FIRST, then stop physics
        // This prevents FixedUpdate from running and re-applying velocity

        // 1. Force Animator to Idle state (prevents moving animation)
        var animator = player.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isJumping", false);
        }

        // 2. Change state to Idle to stop all state-based movement
        var stateMachine = player.GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerState.Idle);
        }

        // 3. Disable input (no new input accepted)
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        // 4. Disable movement component (no FixedUpdate execution)
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // 5. Freeze horizontal movement but ALLOW vertical (gravity/fall)
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

        // 1. Force stop ALL physics completely
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // CRITICAL: Zero out velocity BEFORE restoring constraints
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;

            // Then restore constraints
            rb.constraints = _originalConstraints;
        }

        // 2. Force State Machine back to Idle
        var stateMachine = player.GetComponent<PlayerStateMachine>();
        if (stateMachine != null)
        {
            stateMachine.ChangeState(PlayerState.Idle);
        }

        // 3. Clear input buffer and re-enable
        var playerInput = player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.ClearInput(); // Clear cached input BEFORE enabling
            playerInput.enabled = true;

            // Force emit OnMoveStopped event to trigger animation
            EventBus.Emit(PlayerActionEventType.OnMoveStopped,
                new MovementEventData
                {
                    Velocity = Vector2.zero,
                    Direction = 0, // int, not Vector2
                    IsGrounded = true,
                    Speed = 0f
                });
        }        // 4. Force reset animator to Idle
        var animator = player.GetComponentInChildren<Animator>();
        if (animator != null)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isJumping", false);
        }

        // 5. Re-enable movement LAST (after everything is clean)
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}