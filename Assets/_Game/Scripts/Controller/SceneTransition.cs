using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneTransitionType
{
    OnPlayerDied,
}

/// <summary>
/// Quản lý scene transition với overlay màu đen có hiệu ứng di chuyển
/// Đặt trên Canvas của mỗi scene, không dùng DontDestroyOnLoad
/// </summary>
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }
    public GameObject parentCanvas;
    [Header("Transition Settings")]
    [SerializeField] private RectTransform overlayPanel; // Panel màu đen overlay
    [SerializeField] private float transitionDuration = 0.5f; // Thời gian hiệu ứng
    
    [Header("Slide Settings")]
    [SerializeField] private TransitionDirection slideDirection = TransitionDirection.FromRight;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector2 _hiddenPosition;
    private Vector2 _hiddenPositionOpposite; // Vị trí ẩn đối diện (cho slide out)
    private Vector2 _visiblePosition;
    private bool _isTransitioning = false;
    
    public enum TransitionDirection
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    private void Awake()
    {
        Instance = this;
        CalculatePositions();
        
        // Ẩn overlay ban đầu
        if (overlayPanel != null)
        {
            overlayPanel.anchoredPosition = _hiddenPosition;
        }
        DontDestroyOnLoad(parentCanvas);
        DontDestroyOnLoad(this.gameObject);
    }
    
    private void CalculatePositions()
    {
        if (overlayPanel == null) return;
        
        // Position khi overlay hiển thị (che toàn màn hình)
        _visiblePosition = Vector2.zero;
        
        // Position khi overlay ẩn (ngoài màn hình)
        Canvas canvas = overlayPanel.GetComponentInParent<Canvas>();
        if (canvas == null) return;
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float screenWidth = canvasRect.rect.width;
        float screenHeight = canvasRect.rect.height;
        
        switch (slideDirection)
        {
            case TransitionDirection.FromLeft:
                _hiddenPosition = new Vector2(-screenWidth, 0);
                _hiddenPositionOpposite = new Vector2(screenWidth, 0); // Slide out sang phải
                break;
            case TransitionDirection.FromRight:
                _hiddenPosition = new Vector2(screenWidth, 0);
                _hiddenPositionOpposite = new Vector2(-screenWidth, 0); // Slide out sang trái
                break;
            case TransitionDirection.FromTop:
                _hiddenPosition = new Vector2(0, screenHeight);
                _hiddenPositionOpposite = new Vector2(0, -screenHeight); // Slide out xuống dưới
                break;
            case TransitionDirection.FromBottom:
                _hiddenPosition = new Vector2(0, -screenHeight);
                _hiddenPositionOpposite = new Vector2(0, screenHeight); // Slide out lên trên
                break;
        }
    }
    
    /// <summary>
    /// Load scene với hiệu ứng transition
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneName));
        }
    }
    
    /// <summary>
    /// Load scene với hiệu ứng transition (sử dụng build index)
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        if (!_isTransitioning)
        {
            StartCoroutine(TransitionToScene(sceneIndex));
        }
    }
    
    /// <summary>
    /// Coroutine xử lý transition sequence
    /// </summary>
    private IEnumerator TransitionToScene(string sceneName)
    {
        _isTransitioning = true;
        
        // Phase 1: Slide overlay vào (che màn hình)
        yield return StartCoroutine(SlideIn());
        
        // Phase 2: Load scene
        SceneManager.LoadScene(sceneName);
        EventBus.Clear();

        yield return StartCoroutine(SlideOut());

    }

    public IEnumerator TransitionResetScene(Player player)
    {
        _isTransitioning = true;
        
        // Phase 1: Slide overlay vào
        yield return StartCoroutine(SlideIn());
        Time.timeScale = 1f;

        
        EventBus.Emit(SceneTransitionType.OnPlayerDied, player);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SlideOut());

        player.IsDead = false;

    }
    
    /// <summary>
    /// Coroutine xử lý transition sequence (build index)
    /// </summary>
    private IEnumerator TransitionToScene(int sceneIndex)
    {
        _isTransitioning = true;
        
        // Phase 1: Slide overlay vào
        yield return StartCoroutine(SlideIn());
        
        // Phase 2: Load scene
        SceneManager.LoadScene(sceneIndex);
        EventBus.Clear();
    }
    
    /// <summary>
    /// Slide overlay vào màn hình (che toàn bộ)
    /// </summary>
    private IEnumerator SlideIn()
    {
        if (overlayPanel == null) yield break;
        overlayPanel.gameObject.SetActive(true);
        
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Dùng unscaledDeltaTime để không bị ảnh hưởng bởi timeScale
            float t = elapsed / transitionDuration;
            float curveValue = slideCurve.Evaluate(t);
            
            overlayPanel.anchoredPosition = Vector2.Lerp(_hiddenPosition, _visiblePosition, curveValue);
            
            yield return null;
        }
        
        overlayPanel.anchoredPosition = _visiblePosition;
    }
    
    /// <summary>
    /// Slide overlay ra khỏi màn hình (hiện scene) - gọi từ scene mới
    /// </summary>
    public void SlideOutOnSceneLoad()
    {
        StartCoroutine(SlideOut());
    }
    
    private IEnumerator SlideOut()
    {
        if (overlayPanel == null) yield break;
        
        // Overlay bắt đầu ở vị trí visible (che màn hình)
        overlayPanel.anchoredPosition = _visiblePosition;
        
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionDuration;
            float curveValue = slideCurve.Evaluate(t);
            
            // Slide ra phía đối diện (cùng hướng với slide in)
            overlayPanel.anchoredPosition = Vector2.Lerp(_visiblePosition, _hiddenPositionOpposite, curveValue);
            
            yield return null;
        }
        
        overlayPanel.anchoredPosition = _hiddenPositionOpposite;
        
        // Reset về vị trí ban đầu để sẵn sàng cho lần slide tiếp theo
        overlayPanel.anchoredPosition = _hiddenPosition;
        _isTransitioning = false;
    }
}
