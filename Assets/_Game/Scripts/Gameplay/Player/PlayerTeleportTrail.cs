using UnityEngine;

/// <summary>
/// Quản lý trail effect khi player teleport
/// Tạo line từ vị trí cũ đến vị trí mới, fade dần từ đầu đến cuối
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class PlayerTeleportTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    [Tooltip("Thời gian trail fade out hoàn toàn")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    [Tooltip("Độ rộng trail")]
    [SerializeField] private float trailWidth = 0.3f;
    
    [Tooltip("Màu trail")]
    [SerializeField] private Color trailColor = new Color(0f, 1f, 1f, 1f); // Cyan
    
    [Tooltip("Số điểm trên line (càng nhiều càng smooth)")]
    [SerializeField] private int lineSegments = 20;
    
    private LineRenderer _lineRenderer;
    private float _fadeTimer;
    private bool _isFading;
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        ConfigureLineRenderer();
        
        // Ẩn line ban đầu
        _lineRenderer.enabled = false;
    }

    void Update()
    {
        if (_isFading)
        {
            _fadeTimer += Time.deltaTime;
            float fadeProgress = _fadeTimer / fadeDuration;
            
            if (fadeProgress >= 1f)
            {
                // Fade hoàn tất
                _lineRenderer.enabled = false;
                _isFading = false;
            }
            else
            {
                // Fade dần từ điểm đầu đến điểm cuối
                UpdateLineFade(fadeProgress);
            }
        }
    }

    /// <summary>
    /// Configure line renderer settings
    /// </summary>
    private void ConfigureLineRenderer()
    {
        if (_lineRenderer == null) return;

        // Cấu hình cơ bản
        _lineRenderer.startWidth = trailWidth;
        _lineRenderer.endWidth = trailWidth;
        
        // Material và color
        _lineRenderer.startColor = trailColor;
        _lineRenderer.endColor = trailColor;
        
        // Rendering settings
        _lineRenderer.numCornerVertices = 5;
        _lineRenderer.numCapVertices = 5;
        _lineRenderer.useWorldSpace = true;
        
        // Sorting
        _lineRenderer.sortingLayerName = "Default";
        _lineRenderer.sortingOrder = 10;
        
        // Số điểm
        _lineRenderer.positionCount = lineSegments;
    }

    /// <summary>
    /// Kích hoạt trail từ vị trí cũ đến vị trí mới
    /// </summary>
    public void ActivateTrail(Vector3 startPos, Vector3 endPos)
    {
        if (_lineRenderer == null) return;

        _startPosition = startPos;
        _endPosition = endPos;
        
        // Tạo line từ start đến end
        CreateLine();
        
        // Bắt đầu fade
        _lineRenderer.enabled = true;
        _isFading = true;
        _fadeTimer = 0f;
    }

    /// <summary>
    /// Tạo line từ start đến end với nhiều segments
    /// </summary>
    private void CreateLine()
    {
        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1);
            Vector3 point = Vector3.Lerp(_startPosition, _endPosition, t);
            _lineRenderer.SetPosition(i, point);
        }
    }

    /// <summary>
    /// Update fade effect - thu dần về điểm đích (end position)
    /// </summary>
    private void UpdateLineFade(float fadeProgress)
    {
        // fadeProgress = 0: line đầy đủ từ start → end
        // fadeProgress = 1: line thu hết về điểm end
        
        // Tính điểm bắt đầu mới (di chuyển dần từ start về end)
        Vector3 currentStart = Vector3.Lerp(_startPosition, _endPosition, fadeProgress);
        
        // Update tất cả điểm trên line để tạo hiệu ứng "rút ngắn"
        for (int i = 0; i < lineSegments; i++)
        {
            float t = i / (float)(lineSegments - 1);
            Vector3 point = Vector3.Lerp(currentStart, _endPosition, t);
            _lineRenderer.SetPosition(i, point);
        }
        
        // Fade alpha nhẹ để mượt hơn
        Color startColor = trailColor;
        startColor.a = 1f - fadeProgress * 0.5f; // Giữ một phần alpha
        
        Color endColor = trailColor;
        endColor.a = 1f;
        
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

    /// <summary>
    /// Clear trail ngay lập tức
    /// </summary>
    public void ClearTrail()
    {
        if (_lineRenderer == null) return;

        _lineRenderer.enabled = false;
        _isFading = false;
    }

    /// <summary>
    /// Điều chỉnh màu trail runtime
    /// </summary>
    public void SetTrailColor(Color newColor)
    {
        trailColor = newColor;
        if (_lineRenderer != null)
        {
            _lineRenderer.startColor = newColor;
            _lineRenderer.endColor = newColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize trail width
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, trailWidth / 2f);
        
        // Visualize line khi đang fade
        if (_isFading)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_startPosition, _endPosition);
        }
    }
}
