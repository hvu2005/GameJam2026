using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneTransitionType
{
    OnPlayerDied,
}

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }
    public GameObject parentCanvas;
    
    [Header("Transition Settings")]
    [SerializeField] private RectTransform overlayPanel; 
    [SerializeField] private float transitionDuration = 0.5f; 
    
    [Header("Slide Settings")]
    [SerializeField] private TransitionDirection slideDirection = TransitionDirection.FromRight;
    [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private Vector2 _hiddenPosition;
    private Vector2 _hiddenPositionOpposite; 
    private Vector2 _visiblePosition = Vector2.zero; // Vị trí hiển thị luôn là 0,0 (giữa màn hình)
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(parentCanvas);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cập nhật vị trí và ẩn ngay khi game bắt đầu
        if (overlayPanel != null)
        {
            UpdateAnimPositions();
            overlayPanel.anchoredPosition = _hiddenPosition;
            overlayPanel.gameObject.SetActive(false); // Tắt đi cho nhẹ
        }
    }

    /// <summary>
    /// Tính toán lại vị trí dựa trên kích thước hiện tại của Panel.
    /// Gọi hàm này trước khi transition để đảm bảo đúng resolution.
    /// </summary>
    private void UpdateAnimPositions()
    {
        if (overlayPanel == null) return;

        // Lấy kích thước thực tế của Panel (thường bằng kích thước màn hình nếu full stretch)
        float width = overlayPanel.rect.width;
        float height = overlayPanel.rect.height;

        switch (slideDirection)
        {
            case TransitionDirection.FromLeft:
                // Từ trái vào -> Bắt đầu ở (-width, 0) -> Kết thúc ở (width, 0)
                _hiddenPosition = new Vector2(-width, 0);
                _hiddenPositionOpposite = new Vector2(width, 0);
                break;

            case TransitionDirection.FromRight:
                // Từ phải vào -> Bắt đầu ở (width, 0) -> Kết thúc ở (-width, 0)
                _hiddenPosition = new Vector2(width, 0);
                _hiddenPositionOpposite = new Vector2(-width, 0);
                break;

            case TransitionDirection.FromTop:
                // Từ trên xuống -> Bắt đầu ở (0, height) -> Kết thúc ở (0, -height)
                _hiddenPosition = new Vector2(0, height);
                _hiddenPositionOpposite = new Vector2(0, -height);
                break;

            case TransitionDirection.FromBottom:
                // Từ dưới lên -> Bắt đầu ở (0, -height) -> Kết thúc ở (0, height)
                _hiddenPosition = new Vector2(0, -height);
                _hiddenPositionOpposite = new Vector2(0, height);
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!_isTransitioning) StartCoroutine(TransitionToScene(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        if (!_isTransitioning) StartCoroutine(TransitionToScene(sceneIndex));
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        _isTransitioning = true;
        yield return StartCoroutine(SlideIn());
        
        SceneManager.LoadScene(sceneName);
        EventBus.Clear();

        yield return StartCoroutine(SlideOut());
    }

    private IEnumerator TransitionToScene(int sceneIndex)
    {
        _isTransitioning = true;
        yield return StartCoroutine(SlideIn());
        
        SceneManager.LoadScene(sceneIndex);
        EventBus.Clear();

        yield return StartCoroutine(SlideOut());
    }

    // Ví dụ hàm reset scene cho Player (giữ nguyên logic của bạn)
    /*
    public IEnumerator TransitionResetScene(Player player)
    {
        _isTransitioning = true;
        yield return StartCoroutine(SlideIn());
        Time.timeScale = 1f;

        // EventBus.Emit(SceneTransitionType.OnPlayerDied, player);

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(SlideOut());

        player.IsDead = false;
    }
    */

    private IEnumerator SlideIn()
    {
        if (overlayPanel == null) yield break;

        // 1. Tính toán lại vị trí ngay lúc này để đảm bảo chính xác với màn hình hiện tại
        UpdateAnimPositions();

        overlayPanel.gameObject.SetActive(true);
        
        // 2. Đặt vị trí bắt đầu (Ngoài màn hình)
        overlayPanel.anchoredPosition = _hiddenPosition;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curveValue = slideCurve.Evaluate(t);

            // 3. Di chuyển từ Ngoài vào Giữa (0,0)
            overlayPanel.anchoredPosition = Vector2.LerpUnclamped(_hiddenPosition, _visiblePosition, curveValue);

            yield return null;
        }

        overlayPanel.anchoredPosition = _visiblePosition;
    }

    private IEnumerator SlideOut()
    {
        if (overlayPanel == null) yield break;

        // Đảm bảo đang ở giữa màn hình trước khi slide out
        overlayPanel.anchoredPosition = _visiblePosition;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curveValue = slideCurve.Evaluate(t);

            // 4. Di chuyển từ Giữa ra Ngoài (phía đối diện)
            overlayPanel.anchoredPosition = Vector2.LerpUnclamped(_visiblePosition, _hiddenPositionOpposite, curveValue);

            yield return null;
        }

        overlayPanel.anchoredPosition = _hiddenPositionOpposite;
        
        // Reset về vị trí chờ (ẩn) và tắt object
        overlayPanel.anchoredPosition = _hiddenPosition;
        overlayPanel.gameObject.SetActive(false); 
        _isTransitioning = false;
    }
}