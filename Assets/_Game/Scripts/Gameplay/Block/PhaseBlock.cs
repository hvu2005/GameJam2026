using UnityEngine;
using UnityEngine.Tilemaps;

public class PhaseBlock : MonoBehaviour
{

    [Header("⚙️ Settings")]
    [Tooltip("Chọn nhóm cho block này")]
    [SerializeField] private PhaseType phaseType;

    [Tooltip("Thời gian mỗi pha (Giây)")]
    [SerializeField] private float cycleDuration = 3f;

    [Tooltip("Offset thời gian (nếu muốn lệch pha một chút, mặc định để 0)")]
    [SerializeField] private float timeOffset = 0f;

    [Header("🎨 Visuals")]
    [SerializeField] private Sprite activeSprite;   // Hình nền đá sáng (Đặc)
    [SerializeField] private Tilemap tilemap;
    
    [Range(0f, 1f)]
    [SerializeField] private float activeAlpha = 1f;
    
    [Range(0f, 1f)]
    [SerializeField] private float inactiveAlpha = 0.3f; // Độ mờ khi tắt

    // Components
    private Collider2D col;
    private SpriteRenderer sr;
    private bool isSolid = true;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        
        // Tự động lấy sprite hiện tại làm active sprite nếu chưa gán
        if (activeSprite == null && sr != null) activeSprite = sr.sprite;
    }

    private void Update()
    {
        // --- LOGIC ĐỒNG BỘ TOÀN CẦU (Global Sync) ---
        // Công thức này giúp tất cả block tự đồng bộ mà không cần Manager
        
        // Tổng thời gian 1 vòng lặp = 3s pha 1 + 3s pha 2 = 6s
        float totalCycle = cycleDuration * 2;
        
        // Tính thời gian hiện tại trong vòng lặp (từ 0 đến 6)
        float timer = (Time.time + timeOffset) % totalCycle;

        // Xác định xem hiện tại đang là lượt của Pha 1 hay Pha 2
        bool isPhase1Active = timer < cycleDuration;

        // --- XỬ LÝ TRẠNG THÁI CỦA BLOCK NÀY ---
        bool shouldBeActive = false;

        if (phaseType == PhaseType.Phase1_Blue)
        {
            shouldBeActive = isPhase1Active;
        }
        else
        {
            shouldBeActive = !isPhase1Active;
        }

        if (shouldBeActive != isSolid)
        {
            UpdateBlockState(shouldBeActive);
        }
    }

    private void UpdateBlockState(bool active)
    {
        isSolid = active;

        // 1. Xử lý va chạm (Logic đứng lên/đi xuyên)
        if (col != null) col.enabled = active;

        // 2. Xử lý hình ảnh (Visual)
        if (tilemap != null)
        {
            // Đổi màu/Alpha
            Color c = tilemap.color;
            c.a = active ? activeAlpha : inactiveAlpha;
            tilemap.color = c;
        }
    }
}