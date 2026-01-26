using UnityEngine;

//Hazard Types
public enum HazardType { Static, Timing, Moving, Chasing, Conditional }
public enum AIState { Idle, Chasing, Charging }
public enum EnemyType { HomingProjectile, Bat }
public enum MoveType { Waypoints, Rotate }
public enum TriggerType { Proximity, Contact }
public enum ActionType { Fall, AppearAttack, Break }