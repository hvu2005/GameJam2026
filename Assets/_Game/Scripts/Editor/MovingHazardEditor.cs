#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MovingHazard))]
public class MovingHazardEditor : Editor
{
    // Hàm này chuyên dùng để vẽ giao diện tương tác trong Scene View
    protected virtual void OnSceneGUI()
    {
        MovingHazard t = (MovingHazard)target;

        // Chỉ hiện khi đang ở chế độ Waypoints
        // Lưu ý: Biến moveType là private nên ta check thông qua list waypoints cho nhanh, 
        // hoặc bạn đổi moveType sang public nếu muốn check kỹ hơn.
        if (t.waypoints == null || t.waypoints.Count == 0) return;

        // Bắt đầu check thay đổi để hỗ trợ Undo (Ctrl+Z)
        EditorGUI.BeginChangeCheck();

        for (int i = 0; i < t.waypoints.Count; i++)
        {
            // 1. Vẽ Label tên điểm
            Handles.Label(t.waypoints[i] + Vector3.up * 0.5f, $"Point {i}");

            // 2. Vẽ cái tay cầm mũi tên (Position Handle)
            // Cho phép kéo thả và trả về vị trí mới
            Vector3 newPos = Handles.PositionHandle(t.waypoints[i], Quaternion.identity);

            // 3. Ghi nhận thay đổi
            if (EditorGUI.EndChangeCheck())
            {
                // Lưu vào hệ thống Undo của Unity
                Undo.RecordObject(t, "Move Waypoint");
                
                // Cập nhật giá trị mới
                t.waypoints[i] = newPos;
                
                // Đánh dấu object đã bị sửa đổi để Unity lưu lại vào Scene
                EditorUtility.SetDirty(t);
            }
        }
    }
}
#endif