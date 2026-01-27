using System.Collections.Generic;
using UnityEngine;

public class MovingHazard : Hazard
{

    [Header("Movement Config")]
    [SerializeField] private MoveType moveType;
    [SerializeField] private float speed = 3f;

    [Header("Waypoint Mode")]
    [Tooltip("Waypoints defining the movement path")]
    public List<Vector3> waypoints = new List<Vector3>();

    [Header("Rotation Config")]
    [SerializeField] private bool rotateWhileMoving = false; // Xoay trong khi di chuyển waypoint
    [SerializeField] private float rotateSpeed = 45f; // Độ/giây

    private int currentPointIndex = 0;

    protected virtual void FixedUpdate()
    {
        if (moveType == MoveType.Waypoints)
        {
            HandleWaypointMovement();
            
            // Xoay đồng thời nếu bật option
            if (rotateWhileMoving)
            {
                HandleRotation();
            }
        }
        else if (moveType == MoveType.Rotate)
        {
            HandleRotation();
        }
    }

    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);
        //Player die ngay khi chạm vào hazard di chuyển
        target.Die();
    }

    private void HandleWaypointMovement()
    {
        if (waypoints == null || waypoints.Count < 2) return;

        Vector3 targetPoint = waypoints[currentPointIndex];

        transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Count;
        }
    }

    private void HandleRotation()
    {
        // Xoay quanh trục Z (cho Laser quét vòng tròn)
        transform.Rotate(0, 0, rotateSpeed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos() {
        if (moveType == MoveType.Waypoints) 
        {
            if (waypoints == null || waypoints.Count == 0) return;

            // Vẽ các waypoint và đường nối
            for (int i = 0; i < waypoints.Count; i++) 
            {
                // Vẽ sphere tại waypoint
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(waypoints[i], 0.3f);

                // Vẽ line đến waypoint tiếp theo
                Gizmos.color = Color.yellow;
                Vector3 nextPoint = waypoints[(i + 1) % waypoints.Count];
                Gizmos.DrawLine(waypoints[i], nextPoint);
            }

            // Vẽ line từ vị trí hiện tại đến target waypoint khi đang chạy
            if (Application.isPlaying && waypoints.Count > 0) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, waypoints[currentPointIndex]);
                
                // Highlight waypoint đang target
                Gizmos.DrawSphere(waypoints[currentPointIndex], 0.2f);
            }
        }
        
        else if (moveType == MoveType.Rotate) 
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.DrawRay(transform.position, transform.up * 2f);
        }
    }
}