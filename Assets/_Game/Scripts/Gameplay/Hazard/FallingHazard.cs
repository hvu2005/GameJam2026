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

    private Rigidbody2D rb;
    private Vector3 initialPos;
    private bool isFalling = false;
    private Renderer[] renderers;
    private Collider2D col;

    protected void Awake()
    {
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
        yield return transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true)
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
        // Giết Player
        target.Die();

        // Nhũ băng vỡ ngay lập tức
        BreakAndRespawn();
    }

    // Khi chạm vào những thứ KHÁC (như mặt đất)
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); // Giữ logic check Player của cha

        // Nếu chạm đất -> Cũng vỡ
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            BreakAndRespawn();
        }
    }

    // --- LOGIC HỒI PHỤC ---

    private void BreakAndRespawn()
    {
        // Ẩn visual & Tắt va chạm (Player tưởng đã hủy)
        ToggleObject(false);

        // Dừng vật lý ngay lập tức
        SetStaticState();

        // Chờ và hồi phục
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        // Reset vị trí & Hiện lại
        transform.position = initialPos;
        isFalling = false;
        ToggleObject(true);
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