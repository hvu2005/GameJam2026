using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script con quản lý các nút điều khiển trong pause menu (sound, music, return to menu)
/// </summary>
public class IngameSettingsPanel : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string returnSceneName = "GameMenu"; // Scene sẽ load khi nhấn Return to Menu
    
    [Header("UI References")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private bool isUseSceneTransition = false;
    
    private PauseMenuController _pauseController;
    
    void Start()
    {
        // Get parent PauseMenuController reference
        _pauseController = GetComponentInParent<PauseMenuController>();
        Debug.Log($"[IngameSettingsPanel] PauseMenuController found: {_pauseController != null}");
        
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
        
        // Setup back button - gọi ClosePanel của parent controller
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        // Setup return to menu button
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(OnReturnToMenu);
        }
    }
    
    /// <summary>
    /// Xử lý khi nhấn nút Back - đóng panel thông qua parent controller
    /// </summary>
    private void OnBackButtonClicked()
    {
        if (_pauseController != null)
        {
            _pauseController.ClosePanel();
        }
    }
    
    /// <summary>
    /// Xử lý khi nhấn Return to Menu
    /// </summary>
    private void OnReturnToMenu()
    {
        Debug.Log("[IngameSettingsPanel] Return to Menu clicked");
        Time.timeScale = 1f; // Reset timescale trước khi load scene
        
        if (isUseSceneTransition && SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(returnSceneName);
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
            backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        
        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveListener(OnReturnToMenu);
        }
    }
}
