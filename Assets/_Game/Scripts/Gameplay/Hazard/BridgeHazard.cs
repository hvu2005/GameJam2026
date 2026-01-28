using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class BridgeHazard : ConditionalHazard
{
    [Header("Bridge Config")]
    [Tooltip("Tốc độ rơi (Trọng lực)")]
    [SerializeField] private float fallGravity = 2.5f;
    [Tooltip("Thời gian rơi trước khi biến mất hẳn (để không rơi mãi mãi)")]
    [SerializeField] private float fallDuration = 2f;

    [Tooltip("Thời gian chờ để hồi phục lại vị trí cũ (tính từ lúc biến mất)")]
    [SerializeField] private float respawnTime = 3f;

    [Header("Shake Settings")]
    [Tooltip("Độ mạnh của cú rung")]
    [SerializeField] private float shakeStrength = 0.3f;
    [Tooltip("Độ rung (Số lần rung/giây)")]
    [SerializeField] private int shakeVibrato = 20;

    private Rigidbody2D rb;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Renderer[] renderers;
    private Collider2D[] colliders;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lưu lại vị trí gốc để reset
        initialPos = transform.position;
        initialRot = transform.rotation;

        // Lấy tất cả Renderer và Collider để Bật/Tắt
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider2D>();

        SetStaticState();
        OnWarning.AddListener(ShakeBridge);
    }

    private void ShakeBridge()
    {
        transform.DOShakePosition(triggerDelay, shakeStrength, shakeVibrato, 90f, false, true);
    }

    // Hàm này tự chạy sau khi hết thời gian Warning (rung lắc)
    protected override void PerformHazardAction()
    {
        transform.DOKill();
        transform.position = initialPos;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX; // Chỉ rơi thẳng xuống

        // 2. Chạy quy trình hồi phục
        StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        // Cho phép rơi trong X giây (visualize cảnh rơi)
        yield return new WaitForSeconds(fallDuration);

        // --- GIAI ĐOẠN BIẾN MẤT ---
        // Tắt hình ảnh và va chạm (Player tưởng là đã mất hẳn)
        ToggleVisualAndPhysics(false);

        // Dừng vật lý để không rơi nữa
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // --- GIAI ĐOẠN CHỜ HỒI PHỤC ---
        yield return new WaitForSeconds(respawnTime);

        // --- GIAI ĐOẠN HỒI PHỤC ---
        ResetHazard();
    }

    public override void ResetHazard()
    {
        transform.DOKill();
        base.ResetHazard();

        // Đặt lại vị trí cũ
        transform.position = initialPos;
        transform.rotation = initialRot;

        // Reset vật lý về trạng thái đứng yên
        SetStaticState();

        // Hiện lại hình ảnh
        ToggleVisualAndPhysics(true);
    }

    private void SetStaticState()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void ToggleVisualAndPhysics(bool isActive)
    {
        foreach (var r in renderers) r.enabled = isActive;
        foreach (var c in colliders) c.enabled = isActive;
    }
    private void OnDestroy()
    {
        // Dọn dẹp Tween khi object bị hủy để tránh lỗi
        transform.DOKill();
    }
}