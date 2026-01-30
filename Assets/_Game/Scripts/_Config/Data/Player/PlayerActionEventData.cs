using UnityEngine;

public struct MovementEventData
{
    public Vector2 Velocity;
    public int Direction;
    public bool IsGrounded;
    public float Speed;
}

public struct JumpEventData
{
    public int JumpCount;
    public bool IsDoubleJump;
    public float JumpForce;
    public bool UsedCoyoteTime;
    public Vector3 Position;
}

public struct DashEventData
{
    public Vector2 DashDirection;
    public float DashSpeed;
    public float CooldownRemaining;
    public Vector3 StartPosition;
}

public struct StateChangeEventData
{
    public PlayerState FromState;
    public PlayerState ToState;
    public float Timestamp;
}

public struct SkillActivatedData
{
    public int FormID;
    public string SkillName;
}
