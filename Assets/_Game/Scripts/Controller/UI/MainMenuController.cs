using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý Main Menu với các button: Start Game, Settings, Exit Game
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "GamePlay"; // Scene sẽ load khi Start Game

    [Header("UI References")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private SettingsPanel settingsPanel;
    [SerializeField] private bool useSceneTransition = false;

    void Awake()
    {
    }

    void Start()
    {
        // Setup button listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(OnStartGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnOpenSettings);
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(OnExitGame);
        }
    }

    /// <summary>
    /// Xử lý khi nhấn Start Game
    /// </summary>
    private void OnStartGame()
    {

        Debug.Log("Start Game clicked");
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
    /// Xử lý khi nhấn Settings
    /// </summary>
    private void OnOpenSettings()
    {
        Debug.Log("Settings clicked");

        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Xử lý khi nhấn Back trong Settings
    /// </summary>
    private void OnCloseSettings()
    {
        Debug.Log("Back clicked");

        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Xử lý khi nhấn Exit Game
    /// </summary>
    private void OnExitGame()
    {
        Debug.Log("Exit Game clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void OnDestroy()
    {
        // Cleanup listeners
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveListener(OnStartGame);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OnOpenSettings);
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.RemoveListener(OnExitGame);
        }
    }
}