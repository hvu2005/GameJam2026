using UnityEngine;
using UnityEngine.Events;

public class HazardTrigger : MonoBehaviour {
    [SerializeField] private string triggerId; // ID để khớp với Hazard
    [SerializeField] private bool oneTimeUse = true;
    
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (hasTriggered && oneTimeUse) return;

        if (collision.GetComponent<Player>() != null) {
            hasTriggered = true;
            
            // Dùng EventBus để bắn tín hiệu kích hoạt
            // Chúng ta dùng ID dạng string hash hoặc enum để định danh trigger cụ thể
            // EventBus.Emit("TRIGGER_ACTIVATE", triggerId);
            
            // Visual feedback (ví dụ: công tắc bị lún xuống)
            transform.localScale = new Vector3(1, 0.5f, 1);
        }
    }
}