using UnityEngine;

public class MovingHazard : Hazard {

    [Header("Movement Config")]
    [SerializeField] private MoveType moveType;
    [SerializeField] private float speed = 3f;
    
    [Header("Waypoint Mode")]
    [SerializeField] private Transform[] points;
    
    [Header("Rotate Mode (Laser quét)")]
    [SerializeField] private float rotateSpeed = 45f; // Độ/giây

    private int currentPointIndex = 0;

    private void FixedUpdate() {
        if (moveType == MoveType.Waypoints) {
            HandleWaypointMovement();
        } else if (moveType == MoveType.Rotate) {
            HandleRotation();
        }
    }

    private void HandleWaypointMovement() {
        if (points.Length < 2) return;

        Transform target = points[currentPointIndex];
        // Dùng Rigidbody MovePosition để vật lý ổn định hơn (nếu có RB)
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f) {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }

    private void HandleRotation() {
        // Xoay quanh trục Z (cho Laser quét vòng tròn)
        transform.Rotate(0, 0, rotateSpeed * Time.fixedDeltaTime);
    }
}