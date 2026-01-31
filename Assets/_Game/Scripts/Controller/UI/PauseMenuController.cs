using UnityEngine;

/// <summary>
/// Script cha quản lý việc bật/tắt pause menu thông qua input Pause
/// </summary>
public class PauseMenuController : EventTarget
{
    [Header("Panel Reference")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    private PlayerInput _input;
    
    void Start()
    {
        // Get PlayerInput reference
        _input = FindObjectOfType<PlayerInput>();
        Debug.Log($"[PauseMenuController] PlayerInput found: {_input != null}");
        
        // Subscribe to Pause input
        if (_input != null)
        {
            _input.On<bool>(PlayerInputType.Pause, OnPauseInput);
            Debug.Log("[PauseMenuController] Subscribed to Pause input");
        }
        
        // Đảm bảo panel đóng khi start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Xử lý input Pause để toggle panel
    /// </summary>
    private void OnPauseInput(bool value)
    {
        Debug.Log($"[PauseMenuController] OnPauseInput called! Panel active: {pauseMenuPanel?.activeSelf}");
        
        if (pauseMenuPanel == null) return;
        
        if (pauseMenuPanel.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }
    
    /// <summary>
    /// Mở pause menu
    /// </summary>
    public void OpenPanel()
    {
        Debug.Log("[PauseMenuController] Opening panel - pausing game");
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }
    
    /// <summary>
    /// Đóng pause menu
    /// </summary>
    public void ClosePanel()
    {
        Debug.Log("[PauseMenuController] Closing panel - resuming game");
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from input
        if (_input != null)
        {
            _input.Off<bool>(PlayerInputType.Pause, OnPauseInput);
        }
    }
}
