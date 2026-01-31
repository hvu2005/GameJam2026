using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Quản lý load scene với loading screen và animation
/// Dùng một lần cho một scene cụ thể
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad = ""; // Tên scene cần load
    [SerializeField] private bool loadOnStart = true; // Tự động load khi Start
    [SerializeField] private float delayBeforeLoad = 2f; // Thời gian delay trước khi load (giây)
    
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingContainer;
    [SerializeField] private Animator loadingAnimator;
    
    [Header("Settings")]
    [SerializeField] private float minimumLoadingTime = 2f; // Thời gian tối thiểu hiện loading
    [SerializeField] private string loadingTrigger = "Show"; // Trigger animation
    
    void Awake()
    {
        // Ẩn loading container ban đầu nếu không auto load
        if (loadingContainer != null && !loadOnStart)
        {
            loadingContainer.SetActive(false);
        }
    }
    
    void Start()
    {
        // Tự động load scene khi Start
        if (loadOnStart)
        {
            StartCoroutine(AutoLoadOnStart());
        }
    }
    
    /// <summary>
    /// Tự động load scene khi Start với delay
    /// </summary>
    private IEnumerator AutoLoadOnStart()
    {
        // Hiện loading container ngay lập tức
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(true);
        }
        
        // Trigger animation ngay
        if (loadingAnimator != null && !string.IsNullOrEmpty(loadingTrigger))
        {
            loadingAnimator.SetTrigger(loadingTrigger);
        }
        
        // Đợi delay trước khi load scene
        yield return new WaitForSeconds(delayBeforeLoad);
        
        // Bắt đầu load scene
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("⚠️ Scene To Load chưa được thiết lập!");
            yield break;
        }
        
        yield return StartCoroutine(LoadSceneCoroutineInternal(sceneToLoad));
    }
    
    /// <summary>
    /// Load scene đã được config trong Inspector
    /// </summary>
    public void LoadConfiguredScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("⚠️ Scene To Load chưa được thiết lập!");
            return;
        }
        
        LoadScene(sceneToLoad);
    }
    
    /// <summary>
    /// Load scene theo tên
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    /// <summary>
    /// Load scene theo build index
    /// </summary>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneCoroutine(sceneIndex));
    }
    
    /// <summary>
    /// Reload scene hiện tại
    /// </summary>
    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneCoroutine(currentSceneIndex));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Hiện loading container
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(true);
        }
        
        // Trigger animation
        if (loadingAnimator != null && !string.IsNullOrEmpty(loadingTrigger))
        {
            loadingAnimator.SetTrigger(loadingTrigger);
        }
        
        yield return StartCoroutine(LoadSceneCoroutineInternal(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutineInternal(string sceneName)
    {
        // Đợi một khoảng thời gian để animation chạy
        float startTime = Time.time;
        
        // Bắt đầu load scene async
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Không kích hoạt scene ngay
        
        // Đợi loading hoàn tất HOẶC đạt minimum time
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float elapsedTime = Time.time - startTime;
            
            // Khi progress đạt 90% và đã qua minimum time
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Ẩn loading container sau khi load xong
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(false);
        }
    }
    
    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // Hiện loading container
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(true);
        }
        
        // Trigger animation
        if (loadingAnimator != null && !string.IsNullOrEmpty(loadingTrigger))
        {
            loadingAnimator.SetTrigger(loadingTrigger);
        }
        
        // Đợi một khoảng thời gian để animation chạy
        float startTime = Time.time;
        
        // Bắt đầu load scene async
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false; // Không kích hoạt scene ngay
        
        // Đợi loading hoàn tất HOẶC đạt minimum time
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float elapsedTime = Time.time - startTime;
            
            // Khi progress đạt 90% và đã qua minimum time
            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadingTime)
            {
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Ẩn loading container sau khi load xong
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(false);
        }
    }
    
    /// <summary>
    /// Load scene đơn giản không có minimum time (chỉ đợi animation)
    /// </summary>
    public void LoadSceneSimple(string sceneName, float animationDuration = 1f)
    {
        StartCoroutine(LoadSceneSimpleCoroutine(sceneName, animationDuration));
    }
    
    private IEnumerator LoadSceneSimpleCoroutine(string sceneName, float animationDuration)
    {
        // Hiện loading container
        if (loadingContainer != null)
        {
            loadingContainer.SetActive(true);
        }
        
        // Trigger animation
        if (loadingAnimator != null && !string.IsNullOrEmpty(loadingTrigger))
        {
            loadingAnimator.SetTrigger(loadingTrigger);
        }
        
        // Đợi animation chạy
        yield return new WaitForSeconds(animationDuration);
        
        // Load scene
        SceneManager.LoadScene(sceneName);
    }
}
