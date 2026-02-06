using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý Map Selection Panel để player chọn map muốn chơi
/// </summary>
public class MapSelectionPanel : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GamePlay";
    
    [Header("Map Buttons")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button map1Button;
    [SerializeField] private Button map2Button;
    [SerializeField] private Button map3Button;
    [SerializeField] private Button backButton;
    
    [Header("Map Index Settings")]
    [Tooltip("Index của các map trong MapDatabase (Tutorial=0, Map1=1, Map2=2, Map3=3)")]
    [SerializeField] private int tutorialIndex = 0;
    [SerializeField] private int map1Index = 1;
    [SerializeField] private int map2Index = 2;
    [SerializeField] private int map3Index = 3;
    
    private const string PLAYER_PREFS_LEVEL_KEY = "CurrentLevelIndex";
    
    void Start()
    {
        // Setup map selection buttons
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(() => OnMapSelected(tutorialIndex));
        }
        
        if (map1Button != null)
        {
            map1Button.onClick.AddListener(() => OnMapSelected(map1Index));
        }
        
        if (map2Button != null)
        {
            map2Button.onClick.AddListener(() => OnMapSelected(map2Index));
        }
        
        if (map3Button != null)
        {
            map3Button.onClick.AddListener(() => OnMapSelected(map3Index));
        }
        
        // Setup back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(ClosePanel);
        }
    }
    
    /// <summary>
    /// Xử lý khi player chọn map
    /// </summary>
    private void OnMapSelected(int mapIndex)
    {
        Debug.Log($"Map {mapIndex} selected");
        
        // Lưu map index vào PlayerPrefs để SimpleMapLoader load đúng map
        PlayerPrefs.SetInt(PLAYER_PREFS_LEVEL_KEY, mapIndex);
        PlayerPrefs.Save();
        
        // Load game scene
        if (SceneTransition.Instance != null)
        {
            SceneTransition.Instance.LoadScene(gameSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }
    
    /// <summary>
    /// Mở map selection panel
    /// </summary>
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Đóng map selection panel
    /// </summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
    
    void OnDestroy()
    {
        // Cleanup listeners
        if (tutorialButton != null)
        {
            tutorialButton.onClick.RemoveAllListeners();
        }
        
        if (map1Button != null)
        {
            map1Button.onClick.RemoveAllListeners();
        }
        
        if (map2Button != null)
        {
            map2Button.onClick.RemoveAllListeners();
        }
        
        if (map3Button != null)
        {
            map3Button.onClick.RemoveAllListeners();
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(ClosePanel);
        }
    }
}
