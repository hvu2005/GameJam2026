using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingHazard : Hazard
{
    [Header("Falling Config")]
    [Tooltip("Khoảng cách phát hiện Player bên dưới")]
    [SerializeField] private Vector2 detectionSize = new Vector2(1f, 0.5f);
    [SerializeField] private float detectionLength = 5f;

    [Tooltip("Layer của Player (để Raycast nhận diện)")]
    [SerializeField] private LayerMask playerLayer;

    [Tooltip("Layer của Đất (để nhũ băng vỡ khi chạm đất)")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Physics")]
    [Tooltip("Tốc độ rơi")]
    [SerializeField] private float fallGravity = 3f;

    [Header("Respawn Config")]
    [Tooltip("Thời gian chờ để hồi phục sau khi vỡ")]
    [SerializeField] private float respawnTime = 3f;
    [Tooltip("Thời gian rung lắc trước khi rơi")]
    [SerializeField] private float shakeDuration = 0.5f;
    [Tooltip("Độ mạnh của cú rung (Biên độ)")]
    [SerializeField] private float shakeStrength = 0.5f;
    [Tooltip("Độ rung (Số lần rung/giây)")]
    [SerializeField] private int shakeVibrato = 20;
    [Tooltip("Độ ngẫu nhiên (0-90)")]
    [SerializeField] private float shakeRandomness = 90f;
    [SerializeField] private bool isUsedShake = true;

    [Header("Animation")]
    [SerializeField] private string breakTriggerName = "Destroy";
    [SerializeField] private string idleStateName = "Idle"; // Tên state idle trong Animator
    [SerializeField] private string breakStateName = "CeilingHazard"; // Tên state trong Animator
    [SerializeField] private float breakAnimationDuration = 0.5f; // Thời gian animation vỡ (fallback nếu không tìm được)
    private Animator animator;

    private Rigidbody2D rb;
    private Vector3 initialPos;
    private bool isFalling = false;
    private bool isBreaking = false; // Đang trong quá trình vỡ
    private Renderer[] renderers;
    private Collider2D col;

    protected void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        renderers = GetComponentsInChildren<Renderer>();

        // Lưu vị trí gốc
        initialPos = transform.position;

        // Trạng thái ban đầu: Treo lơ lửng
        SetStaticState();
    }

    private void Update()
    {
        // Nếu chưa rơi -> Bắn tia Raycast xuống dưới để tìm Player
        if (!isFalling)
        {
            DetectPlayerBelow();
        }
    }

    private void DetectPlayerBelow()
    {
        // Bắn tia từ vị trí nhũ băng, hướng xuống dưới
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            detectionSize,
            0f,
            Vector2.down,
            detectionLength,
            playerLayer
        );

        if (hit.collider != null)
        {
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        isFalling = true;

        // 1. Rung lắc cảnh báo (Game Feel)
        if (isUsedShake)
            yield return transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness)
                              .WaitForCompletion();

        transform.position = initialPos; // Đảm bảo vị trí trở lại chính xác

        // 2. Rơi tự do
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
    }

    // --- XỬ LÝ VA CHẠM (Override từ Base Class) ---

    // Khi chạm vào Player -> Base class gọi hàm này
    protected override void OnActivate(PlayerEntity target)
    {
        if (isBreaking) return; // Tránh trigger nhiều lần
        
        // Giết Player
        target.Die();

        // Nhũ băng vỡ
        BreakAndRespawn();
    }

    // Khi chạm vào những thứ KHÁC (như mặt đất)
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); // Giữ logic check Player của cha

        // Nếu chạm đất -> Cũng vỡ
        if (!isBreaking && ((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            BreakAndRespawn();
        }
    }

    // --- LOGIC HỒI PHỤC ---

    private void BreakAndRespawn()
    {
        isBreaking = true;
        
        // Dừng vật lý ngay lập tức
        SetStaticState();
        
        // Tắt collider để không va chạm nữa
        if (col) col.enabled = false;

        // Trigger animation vỡ
        if (animator != null && !string.IsNullOrEmpty(breakTriggerName))
        {
            animator.SetTrigger(breakTriggerName);
        }

        // Chờ và hồi phục
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Đợi animation vỡ chạy xong
        yield return new WaitForSeconds(GetBreakAnimationDuration());
        
        // Ẩn visual SAU KHI animation chạy xong
        foreach (var r in renderers) r.enabled = false;
        
        // Đợi thời gian respawn
        yield return new WaitForSeconds(respawnTime);

        // Reset vị trí & trạng thái
        transform.position = initialPos;
        transform.rotation = Quaternion.identity; // Reset rotation nếu bị xoay
        isFalling = false;
        isBreaking = false;
        
        // Reset animator về trạng thái idle
        ResetAnimator();
        
        // Hiện lại visual và collider
        ToggleObject(true);
        
        // Reset physics state
        SetStaticState();
    }
    
    /// <summary>
    /// Reset animator về trạng thái ban đầu
    /// </summary>
    private void ResetAnimator()
    {
        if (animator == null) return;
        
        // Reset tất cả triggers để tránh trigger cũ còn active
        if (!string.IsNullOrEmpty(breakTriggerName))
        {
            animator.ResetTrigger(breakTriggerName);
        }
        
        // Force play state idle nếu có
        if (!string.IsNullOrEmpty(idleStateName))
        {
            animator.Play(idleStateName, 0, 0f); // Layer 0, normalized time 0
        }
        
        // Update animator để apply changes ngay
        animator.Update(0f);
    }
    
    /// <summary>
    /// Lấy độ dài animation vỡ từ Animator
    /// </summary>
    private float GetBreakAnimationDuration()
    {
        // Nếu không có animator hoặc không config state name → dùng fallback
        if (animator == null || string.IsNullOrEmpty(breakStateName))
            return breakAnimationDuration;
        
        // Tìm AnimationClip trong state
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        if (ac == null || ac.animationClips == null || ac.animationClips.Length == 0)
            return breakAnimationDuration;
        
        foreach (AnimationClip clip in ac.animationClips)
        {
            // Tìm clip có tên khớp với state (hoặc trigger)
            if (clip.name.Contains(breakStateName) || clip.name.Contains(breakTriggerName))
            {
                return clip.length;
            }
        }
        
        // Fallback về giá trị config nếu không tìm thấy clip
        return breakAnimationDuration;
    }

    private void SetStaticState()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private void ToggleObject(bool state)
    {
        if (col) col.enabled = state;
        foreach (var r in renderers) r.enabled = state;
    }

    // Vẽ Raycast trong Editor để dễ chỉnh độ dài
    private void OnDrawGizmosSelected()
    {
        // 1. Tính toán điểm bắt đầu và kết thúc
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)Vector2.down * detectionLength;

        // Mặc định là màu xanh (An toàn/Đang tìm)
        Gizmos.color = Color.green;

        // --- CHECK THỬ TRONG EDITOR ---
        // Bắn thử 1 cái BoxCast ngay trong Editor để xem có trúng Player không
        // (Giúp bạn debug xem layer/khoảng cách đúng chưa mà không cần Play game)
        RaycastHit2D hit = Physics2D.BoxCast(startPos, detectionSize, 0f, Vector2.down, detectionLength, playerLayer);

        if (hit.collider != null)
        {
            // Nếu trúng Player -> Đổi màu ĐỎ & Rút ngắn độ dài vẽ đến chỗ trúng
            Gizmos.color = Color.red;
            endPos = startPos + (Vector3)Vector2.down * hit.distance;
        }

        // --- VẼ HÌNH HỘP QUÉT (SWEEP) ---

        // 2. Vẽ Hộp tại vị trí Bắt đầu (Trên trần)
        Gizmos.DrawWireCube(startPos, new Vector3(detectionSize.x, detectionSize.y, 0));

        // 3. Vẽ Hộp tại vị trí Kết thúc (Dưới đất hoặc chỗ trúng Player)
        Gizmos.DrawWireCube(endPos, new Vector3(detectionSize.x, detectionSize.y, 0));

        // 4. Vẽ 2 đường nối 2 bên (Thể hiện độ quét xuống)
        float halfWidth = detectionSize.x / 2;
        float halfHeight = detectionSize.y / 2; // Tùy chọn, nối từ mép dưới hộp trên xuống mép trên hộp dưới cho gọn

        // Đường bên trái
        Gizmos.DrawLine(
            startPos + new Vector3(-halfWidth, 0, 0),
            endPos + new Vector3(-halfWidth, 0, 0)
        );

        // Đường bên phải
        Gizmos.DrawLine(
            startPos + new Vector3(halfWidth, 0, 0),
            endPos + new Vector3(halfWidth, 0, 0)
        );

        // Vẽ thêm 1 tia ở giữa cho dễ căn tâm
        Gizmos.color = new Color(1, 1, 1, 0.2f); // Màu trắng mờ
        Gizmos.DrawLine(startPos, endPos);
    }
}