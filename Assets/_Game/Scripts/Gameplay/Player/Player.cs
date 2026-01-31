
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerJump), typeof(PlayerDash))]
[RequireComponent(typeof(PlayerFormController), typeof(PlayerGravityController), typeof(PlayerStateMachine))]
public class Player : PlayerEntity
{
    [Header("Player Configuration")]
    [SerializeField] private PlayerConfig config;
    
    private PlayerMovement _movement;
    private PlayerJump _jump;
    private PlayerDash _dash;
    private PlayerFormController _formController;
    private PlayerStateMachine _stateMachine;
    
    public PlayerMovement Movement => _movement;
    public PlayerJump Jump => _jump;
    public PlayerDash Dash => _dash;
    public PlayerFormController FormController => _formController;
    public PlayerStateMachine StateMachine => _stateMachine;
    public PlayerConfig Config => config;
    
    public bool IsGrounded => _movement != null && _movement.IsGrounded;
    public int FacingDirection => _movement != null ? _movement.FacingDirection : 1;

    public bool IsDead { get; set; } = false;

    void Awake()
    {
        InitializeComponents();
    }

    public void Start()
    {
        FormUnlockManager.UnlockAll();
    }
    
    private void InitializeComponents()
    {
        _movement = GetComponent<PlayerMovement>();
        _jump = GetComponent<PlayerJump>();
        _dash = GetComponent<PlayerDash>();
        _formController = GetComponent<PlayerFormController>();
        _stateMachine = GetComponent<PlayerStateMachine>();
        
        if (config == null)
        {
            Debug.LogError("PlayerConfig is not assigned to Player!", this);
        }
    }

    public override void Die()
    {
        if(IsDead) return;
        IsDead = true;
        // Debug.Log("Player has died.");

        Time.timeScale = 0f;
        Debug.Log("Player has died.");

        ResetPlayerState();

        if (SceneTransition.Instance != null)
            StartCoroutine(SceneTransition.Instance.TransitionResetScene(this));
        else
        {   
            Debug.LogError("SceneTransition instance not found!");
        }

        // EventBus.Clear();
        // SceneManager.LoadScene(this.gameObject.scene.name);
    }

    private void ResetPlayerState()
    {
        if (_jump != null) _jump.CancelJump();
        if (_dash != null) _dash.CancelDash();
        
        if (_dash != null) _dash.ResetCooldown();
        var gc = this.GetComponent<PlayerGravityController>();
        if (gc != null && gc.IsGravityFlipped)
        {
            gc.ToggleGravity();
        }
    }

    public void SetConfig(PlayerConfig newConfig)
    {
        config = newConfig;
    }
}
