using UnityEngine;

/// <summary>
/// ChasingHazard - Hazard truy đuổi player
/// Player chết khi va chạm với hazard này
/// </summary>
public class ChasingHazard : Hazard 
{
    [Header("Chase Config")]
    [Tooltip("Phạm vi phát hiện player")]
    [SerializeField] private float detectRange = 8f;
    
    [Tooltip("Tốc độ di chuyển")]
    [SerializeField] private float moveSpeed = 4f;
    
    [Tooltip("Layer của player để detect")]
    [SerializeField] private LayerMask playerLayer;
    
    [Tooltip("Xoay theo hướng di chuyển")]
    [SerializeField] private bool rotateTowardsPlayer = true;

    private Transform playerTransform;

    private void Update() 
    {
        // Tìm player nếu chưa có
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Di chuyển về phía player
        ChasePlayer();
    }

    /// <summary>
    /// Tìm player trong phạm vi detect
    /// </summary>
    private void FindPlayer() 
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (hit != null && hit.TryGetComponent<PlayerEntity>(out _))
        {
            playerTransform = hit.transform;
        }
    }

    /// <summary>
    /// Truy đuổi player
    /// </summary>
    private void ChasePlayer()
    {
        if (playerTransform == null) return;

        // Di chuyển mượt về phía player
        transform.position = Vector2.MoveTowards(
            transform.position, 
            playerTransform.position, 
            moveSpeed * Time.deltaTime
        );

        // Xoay theo hướng player (optional)
        if (rotateTowardsPlayer)
        {
            Vector3 direction = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    /// <summary>
    /// Player chết khi chạm hazard
    /// </summary>
    protected override void OnActivate(PlayerEntity target)
    {
        base.OnActivate(target);
        target.Die();
    }

    /// <summary>
    /// Visualize detect range trong Scene view
    /// </summary>
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Vẽ line đến player nếu đang chase
        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
}