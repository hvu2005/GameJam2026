using UnityEngine;

namespace _Game.Scripts.Tool
{
    /// <summary>
    /// Component đánh dấu spawn point cho player trong map.
    /// Đặt tag GameObject là "PlayerSpawn" để SimpleMapLoader tìm thấy.
    /// Hướng quay player = localScale.x (dương = phải, âm = trái)
    /// </summary>
    public class PlayerSpawnMarker : MonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Gizmo Settings")]
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private float gizmoSize = 0.5f;

        private void OnDrawGizmos()
        {
            // Draw spawn point sphere
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, gizmoSize);
            
            // Draw facing direction arrow
            int direction = transform.localScale.x >= 0 ? 1 : -1;
            Vector3 arrowStart = transform.position;
            Vector3 arrowEnd = arrowStart + Vector3.right * direction * gizmoSize * 2f;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(arrowStart, arrowEnd);
            
            // Draw arrow head
            Vector3 arrowHeadUp = arrowEnd + new Vector3(-direction * gizmoSize * 0.3f, gizmoSize * 0.3f, 0);
            Vector3 arrowHeadDown = arrowEnd + new Vector3(-direction * gizmoSize * 0.3f, -gizmoSize * 0.3f, 0);
            Gizmos.DrawLine(arrowEnd, arrowHeadUp);
            Gizmos.DrawLine(arrowEnd, arrowHeadDown);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw brighter when selected
            Gizmos.color = gizmoColor * 1.5f;
            Gizmos.DrawSphere(transform.position, gizmoSize);
            
            // Draw position label
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * (gizmoSize + 0.3f),
                $"Player Spawn\n{transform.position}",
                new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.white },
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                }
            );
        }

        private void OnValidate()
        {
            // Reminder để set tag
            if (!gameObject.CompareTag("PlayerSpawn"))
            {
                Debug.LogWarning($"[{gameObject.name}] GameObject cần có tag 'PlayerSpawn' để SimpleMapLoader tìm thấy!", this);
            }
        }
#endif
    }
}
