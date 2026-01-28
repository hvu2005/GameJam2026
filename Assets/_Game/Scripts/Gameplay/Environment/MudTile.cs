using UnityEngine;

/// <summary>
/// Mud Tile - Nền bùn lầy làm chậm player khi đi vào
/// </summary>
public class MudTile : MonoBehaviour
{
    [Header("Mud Effects")]
    [Tooltip("Phần trăm giảm tốc độ di chuyển (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float moveSpeedReduction = 0.5f;
    
    [Tooltip("Phần trăm giảm lực nhảy (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float jumpForceReduction = 0.3f;

    public float MoveSpeedReduction => moveSpeedReduction;
    public float JumpForceReduction => jumpForceReduction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            EventBus.Emit(EnvironmentEventType.EnterMud, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            EventBus.Emit(EnvironmentEventType.ExitMud, this);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize mud area
        Gizmos.color = new Color(0.6f, 0.4f, 0.2f, 0.3f); // Brown color
        if (TryGetComponent<BoxCollider2D>(out var box))
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
    }
}
