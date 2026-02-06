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
    [SerializeField] private Button mapSelectButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private MapSelectionPanel mapSelectionPanel;
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

        if (mapSelectButton != null)
        {
            mapSelectButton.onClick.AddListener(OnOpenMapSelection);
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
    /// Xử lý khi nhấn Map Selection
    /// </summary>
    private void OnOpenMapSelection()
    {
        Debug.Log("Map Selection clicked");

        if (mapSelectionPanel != null)
        {
            mapSelectionPanel.OpenPanel();
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

        if (mapSelectButton != null)
        {
            mapSelectButton.onClick.RemoveListener(OnOpenMapSelection);
        }

        if (exitGameButton != null)
        {
            exitGameButton.onClick.RemoveListener(OnExitGame);
        }
    }
}