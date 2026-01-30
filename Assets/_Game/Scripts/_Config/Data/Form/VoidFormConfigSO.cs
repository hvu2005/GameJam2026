using UnityEngine;

[CreateAssetMenu(fileName = "VoidFormConfig", menuName = "Game/Forms/Void Form", order = 2)]
public class VoidFormConfigSO : BaseFormConfigSO
{
    [Header("Teleport Marker Settings")]
    [Header("Flight Settings")]
    [Tooltip("Initial throw speed")]
    public float markerThrowSpeed = 12f;
    
    [Tooltip("Throw angle offset from facing direction (-90 to 90)")]
    public float markerThrowAngle = 0f;
    
    [Tooltip("Flight force mode: -1 = straight, >0 = parabol multiplier")]
    public float markerFlightForce = -1f;
    
    [Tooltip("Max flight distance for straight mode")]
    public float markerMaxDistance = 10f;
    
    [Tooltip("Acceleration during flight")]
    public float markerAcceleration = 8f;
    
    [Tooltip("Max speed during flight")]
    public float markerMaxSpeed = 20f;
    
    [Header("Timing")]
    [Tooltip("Time window to teleport after marker lands (seconds)")]
    public float teleportWindowTime = 3f;
    
    [Tooltip("Marker lifetime after landing (seconds)")]
    public float markerLifetime = 6f;
    
    [Tooltip("Cooldown after teleport (seconds)")]
    public float teleportCooldown = 9f;
    
    [Header("Teleport")]
    [Tooltip("Pickup radius for collecting marker")]
    public float markerPickupRadius = 0.5f;
    
    [Tooltip("Vertical offset if teleport position blocked")]
    public float teleportOffsetY = 0.5f;
    
    [Tooltip("Max attempts to find valid teleport position")]
    public int teleportMaxAttempts = 15;
    
    [Header("References")]
    public GameObject markerPrefab;
    public LayerMask groundLayer;

    public override bool HasActiveSkill => true;
    public override int GetMaxJumps() => 1;
}
