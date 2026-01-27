
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

    [Header("Teleport Marker Settings")]
    [Header("Flight Settings")]
    [Tooltip("Initial throw speed")]
    [SerializeField] private float markerThrowSpeed = 12f;
    
    [Tooltip("Throw angle offset from facing direction (-90 to 90)")]
    [SerializeField] private float markerThrowAngle = 0f;
    
    [Tooltip("Flight force mode: -1 = straight, >0 = parabol multiplier")]
    [SerializeField] private float markerFlightForce = -1f;
    
    [Tooltip("Max flight distance for straight mode")]
    [SerializeField] private float markerMaxDistance = 10f;
    
    [Tooltip("Acceleration during flight")]
    [SerializeField] private float markerAcceleration = 8f;
    
    [Tooltip("Max speed during flight")]
    [SerializeField] private float markerMaxSpeed = 20f;
    
    [Header("Timing")]
    [Tooltip("Time window to teleport after marker lands (seconds)")]
    [SerializeField] private float teleportWindowTime = 3f;
    
    [Tooltip("Marker lifetime after landing (seconds)")]
    [SerializeField] private float markerLifetime = 6f;
    
    [Tooltip("Cooldown after teleport (seconds)")]
    [SerializeField] private float teleportCooldown = 9f;
    
    [Header("Teleport")]
    [Tooltip("Pickup radius for collecting marker")]
    [SerializeField] private float markerPickupRadius = 0.5f;
    
    [Tooltip("Vertical offset if teleport position blocked")]
    [SerializeField] private float teleportOffsetY = 0.5f;
    
    [Tooltip("Max attempts to find valid teleport position")]
    [SerializeField] private int teleportMaxAttempts = 5;

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
    
    public float MarkerThrowSpeed => markerThrowSpeed;
    public float MarkerThrowAngle => markerThrowAngle;
    public float MarkerFlightForce => markerFlightForce;
    public float MarkerMaxDistance => markerMaxDistance;
    public float MarkerAcceleration => markerAcceleration;
    public float MarkerMaxSpeed => markerMaxSpeed;
    public float TeleportWindowTime => teleportWindowTime;
    public float MarkerLifetime => markerLifetime;
    public float TeleportCooldown => teleportCooldown;
    public float MarkerPickupRadius => markerPickupRadius;
    public float TeleportOffsetY => teleportOffsetY;
    public int TeleportMaxAttempts => teleportMaxAttempts;
    
    public float GroundCheckDistance => groundCheckDistance;
    public Vector2 GroundCheckOffset => groundCheckOffset;
    public LayerMask GroundLayer => groundLayer;
}
