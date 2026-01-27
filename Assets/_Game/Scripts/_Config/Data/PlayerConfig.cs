
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player Config", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    
    [Tooltip("How quickly the player accelerates to max speed")]
    [SerializeField] private float acceleration = 50f;
    
    [Tooltip("How quickly the player decelerates to a stop")]
    [SerializeField] private float deceleration = 50f;
    
    [Tooltip("Movement control multiplier while in the air (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float airControlMultiplier = 0.8f;

    [Header("Jump Settings")]
    [Tooltip("Initial upward velocity for first jump")]
    [SerializeField] private float jumpForce = 15f;
    
    [Tooltip("Upward velocity for double jump")]
    [SerializeField] private float doubleJumpForce = 14f;
    
    [Tooltip("Maximum number of jumps allowed")]
    [SerializeField] private int maxJumps = 2;
    
    [Tooltip("Grace period after leaving ground where jump is still allowed (seconds)")]
    [SerializeField] private float coyoteTime = 0.15f;
    
    [Tooltip("Gravity multiplier when falling (makes falling feel snappier)")]
    [SerializeField] private float fallGravityMultiplier = 2.5f;

    [Header("Dash Settings")]
    [Tooltip("Movement speed during dash")]
    [SerializeField] private float dashSpeed = 25f;
    
    [Tooltip("How long the dash lasts (seconds)")]
    [SerializeField] private float dashDuration = 0.2f;
    
    [Tooltip("Cooldown time between dashes (seconds)")]
    [SerializeField] private float dashCooldown = 1f;
    
    [Tooltip("Can the player dash while in the air?")]
    [SerializeField] private bool canDashInAir = true;

    [Header("Physics Settings")]
    [Tooltip("Distance to check for ground below player")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    
    [Tooltip("Offset from player center for ground check raycast")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);
    
    [Tooltip("Layer mask for detecting ground")]
    [SerializeField] private LayerMask groundLayer;

    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float AirControlMultiplier => airControlMultiplier;
    
    public float JumpForce => jumpForce;
    public float DoubleJumpForce => doubleJumpForce;
    public int MaxJumps => maxJumps;
    public float CoyoteTime => coyoteTime;
    public float FallGravityMultiplier => fallGravityMultiplier;
    
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public bool CanDashInAir => canDashInAir;
    
    public float GroundCheckDistance => groundCheckDistance;
    public Vector2 GroundCheckOffset => groundCheckOffset;
    public LayerMask GroundLayer => groundLayer;
}
