
using UnityEngine;
using System.Collections;

public enum GameState
{
    MainMenu,
    Start,
    Playing,
    Paused,
    WinLevel,
    LoseLevel,
    GameOver
}

public class GameStateManager : EventTarget
{
    // Sử dụng SingleBehaviour thay vì tự implement singleton
    public static GameStateManager Instance => SingleBehaviour.Of<GameStateManager>();

    private StateMachine<GameState> stateMachine;
    
    // Config - có thể điều chỉnh trong code hoặc tạo ScriptableObject sau
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float levelTransitionDelay = 2f;

    public GameState CurrentState => stateMachine?.CurrentState ?? GameState.MainMenu;
    public int CurrentLevel => currentLevel;

    private void Awake()
    {
        InitializeStateMachine();
    }

    private void Update()
    {
        stateMachine?.Update();
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine<GameState>(GameState.MainMenu);

        // Main Menu State
        stateMachine.AddState(
            GameState.MainMenu,
            enter: OnEnterMainMenu,
            update: OnUpdateMainMenu,
            exit: OnExitMainMenu
        );

        // Start State (Khởi tạo level mới)
        stateMachine.AddState(
            GameState.Start,
            enter: OnEnterStart,
            update: OnUpdateStart,
            exit: OnExitStart
        );

        // Playing State
        stateMachine.AddState(
            GameState.Playing,
            enter: OnEnterPlaying,
            update: OnUpdatePlaying,
            exit: OnExitPlaying
        );

        // Paused State
        stateMachine.AddState(
            GameState.Paused,
            enter: OnEnterPaused,
            update: OnUpdatePaused,
            exit: OnExitPaused
        );

        // Win Level State (Chuyển cảnh sau khi thắng)
        stateMachine.AddState(
            GameState.WinLevel,
            enter: OnEnterWinLevel,
            update: OnUpdateWinLevel,
            exit: OnExitWinLevel
        );

        // Lose Level State
        stateMachine.AddState(
            GameState.LoseLevel,
            enter: OnEnterLoseLevel,
            update: OnUpdateLoseLevel,
            exit: OnExitLoseLevel
        );

        // Game Over State
        stateMachine.AddState(
            GameState.GameOver,
            enter: OnEnterGameOver,
            update: OnUpdateGameOver,
            exit: OnExitGameOver
        );
    }

    #region Public Methods - Chuyển State

    public void StartGame()
    {
        currentLevel = 1;
        ChangeState(GameState.Start);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void WinCurrentLevel()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.WinLevel);
        }
    }

    public void LoseCurrentLevel()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.LoseLevel);
        }
    }

    public void ReturnToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void RestartLevel()
    {
        ChangeState(GameState.Start);
    }

    private void ChangeState(GameState newState)
    {
        stateMachine.ChangeState(newState);
        Debug.Log($"[GameStateManager] Changed to state: {newState}");
        
        // Emit event cho các systems khác lắng nghe
        Emit(GameEvent.OnStateChanged, newState);
    }

    #endregion

    #region Main Menu State Handlers

    private void OnEnterMainMenu()
    {
        Debug.Log("[State] Entering Main Menu");
        Time.timeScale = 1f;
        // Logic sẽ được implement sau
    }

    private void OnUpdateMainMenu()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitMainMenu()
    {
        Debug.Log("[State] Exiting Main Menu");
    }

    #endregion

    #region Start State Handlers

    private void OnEnterStart()
    {
        Debug.Log($"[State] Starting Level {currentLevel}");
        Time.timeScale = 1f;
        
        // Logic sẽ được implement sau
        StartCoroutine(StartLevelSequence());
    }

    private IEnumerator StartLevelSequence()
    {
        // Delay trước khi bắt đầu chơi
        yield return new WaitForSeconds(1.5f);
        
        ChangeState(GameState.Playing);
    }

    private void OnUpdateStart()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitStart()
    {
        Debug.Log("[State] Exiting Start");
    }

    #endregion

    #region Playing State Handlers

    private void OnEnterPlaying()
    {
        Debug.Log("[State] Entering Playing");
        Time.timeScale = 1f;
        
        // Logic sẽ được implement sau
    }

    private void OnUpdatePlaying()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitPlaying()
    {
        Debug.Log("[State] Exiting Playing");
    }

    #endregion

    #region Paused State Handlers

    private void OnEnterPaused()
    {
        Debug.Log("[State] Game Paused");
        Time.timeScale = 0f;
        
        // Logic sẽ được implement sau
    }

    private void OnUpdatePaused()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitPaused()
    {
        Debug.Log("[State] Exiting Pause");
        Time.timeScale = 1f;
    }

    #endregion

    #region Win Level State Handlers

    private void OnEnterWinLevel()
    {
        Debug.Log($"[State] Level {currentLevel} Complete!");
        Time.timeScale = 1f;
        
        // Logic sẽ được implement sau
        StartCoroutine(WinLevelSequence());
    }

    private IEnumerator WinLevelSequence()
    {
        // Hiển thị màn hình chiến thắng
        yield return new WaitForSeconds(levelTransitionDelay);
        
        // Hiển thị scene chuyển cảnh
        ShowLevelTransitionScene();
        yield return new WaitForSeconds(2f);
        
        // Tăng level và chuyển sang level tiếp theo
        currentLevel++;
        ChangeState(GameState.Start);
    }

    private void ShowLevelTransitionScene()
    {
        Debug.Log($"[Transition] Showing transition from Level {currentLevel} to Level {currentLevel + 1}");
        
        // Logic hiển thị scene chuyển cảnh sẽ được implement sau
    }

    private void OnUpdateWinLevel()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitWinLevel()
    {
        Debug.Log("[State] Exiting Win Level");
    }

    #endregion

    #region Lose Level State Handlers

    private void OnEnterLoseLevel()
    {
        Debug.Log("[State] Level Failed!");
        Time.timeScale = 1f;
        
        // Logic sẽ được implement sau
        StartCoroutine(LoseLevelSequence());
    }

    private IEnumerator LoseLevelSequence()
    {
        yield return new WaitForSeconds(1.5f);
        
        // Restart level (không có giới hạn mạng)
        Debug.Log("Restarting level...");
        ChangeState(GameState.Start);
    }

    private void OnUpdateLoseLevel()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitLoseLevel()
    {
        Debug.Log("[State] Exiting Lose Level");
    }

    #endregion

    #region Game Over State Handlers

    private void OnEnterGameOver()
    {
        Debug.Log("[State] Game Over!");
        Time.timeScale = 1f;
        
        // Logic sẽ được implement sau
    }

    private void OnUpdateGameOver()
    {
        // Logic sẽ được implement sau
    }

    private void OnExitGameOver()
    {
        Debug.Log("[State] Exiting Game Over");
    }

    #endregion
}