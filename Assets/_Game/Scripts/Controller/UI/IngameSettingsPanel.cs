using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý Settings Panel trong game với sound, music sliders và nút về Main Menu
/// </summary>
public class IngameSettingsPanel : EventTarget
{
    [Header("Scene Settings")]
    [SerializeField] private string returnSceneName = "GameMenu"; // Scene sẽ load khi nhấn Return to Menu
    
    [Header("UI References")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private bool isUseSceneTransition = false;
    
    private PlayerInput _input;
    
    void Start()
    {
        // Get PlayerInput reference
        _input = FindObjectOfType<PlayerInput>();
        Debug.Log($"[IngameSettingsPanel] PlayerInput found: {_input != null}");
        
        // Subscribe to Pause input BEFORE hiding panel
        if (_input != null)
        {
            _input.On<bool>(PlayerInputType.Pause, OnPauseInput);
            Debug.Log("[IngameSettingsPanel] Subscribed to Pause input in Start()");
        }
        
        // Setup sliders
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.AddListener(OnSoundVolumeChanged);
            soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        }
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        
        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(ClosePanel);
        }
        
        // Setup return to menu button
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(OnReturnToMenu);
        }
        
        // Ẩn panel ban đầu
        this.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Xử lý input Pause để toggle panel
    /// </summary>
    private void OnPauseInput(bool value)
    {
        Debug.Log($"[IngameSettingsPanel] OnPauseInput called! Panel active: {gameObject.activeSelf}");
        if (gameObject.activeSelf)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }
    
    /// <summary>
    /// Mở settings panel
    /// </summary>
    public void OpenPanel()
    {
        Debug.Log("[IngameSettingsPanel] Opening panel - pausing game");
        gameObject.SetActive(true);
        Time.timeScale = 0f; // Pause game khi mở settings
    }
    
    /// <summary>
    /// Đóng settings panel
    /// </summary>
    public void ClosePanel()
    {
        Debug.Log("[IngameSettingsPanel] Closing panel - resuming game");
        gameObject.SetActive(false);
        Time.timeScale = 1f; // Resume game khi đóng settings
    }
    
    /// <summary>
    /// Xử lý khi nhấn Return to Menu
    /// </summary>
    private void OnReturnToMenu()
    {
        Debug.Log("Return to Menu clicked");
        Time.timeScale = 1f; // Reset timescale trước khi load scene
        
        SceneTransition transition = FindObjectOfType<SceneTransition>();
        if (transition != null && isUseSceneTransition)
        {
            transition.LoadScene(returnSceneName);
        }
        else
        {
            SceneManager.LoadScene(returnSceneName);
        }
    }
    
    /// <summary>
    /// Xử lý khi thay đổi Sound Volume
    /// </summary>
    private void OnSoundVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        PlayerPrefs.Save();
        
        // TODO: Áp dụng volume cho AudioSource của sound effects
        Debug.Log($"Sound Volume changed to {value}");
    }
    
    /// <summary>
    /// Xử lý khi thay đổi Music Volume
    /// </summary>
    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        
        // TODO: Áp dụng volume cho AudioSource của background music
        Debug.Log($"Music Volume changed to {value}");
    }
    
    void OnDestroy()
    {
        // Cleanup listeners
        if (soundSlider != null)
        {
            soundSlider.onValueChanged.RemoveListener(OnSoundVolumeChanged);
        }
        
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(ClosePanel);
        }
        
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenu);
        }
    }
}
