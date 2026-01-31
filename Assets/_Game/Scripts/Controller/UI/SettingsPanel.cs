using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý Settings Panel với sound và music sliders
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button backButton;
    
    void Start()
    {
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
        
        // Ẩn panel ban đầu
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Mở settings panel
    /// </summary>
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Đóng settings panel
    /// </summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
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
    }
}
