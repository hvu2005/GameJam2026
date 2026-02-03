using System.Collections.Generic;
using UnityEngine;

public class PlayerFormController : MonoBehaviour
{
    [Header("Form Configs")]
    [SerializeField] private DefaultFormConfigSO defaultFormConfig;
    [SerializeField] private AgilityFormConfigSO agilityFormConfig;
    [SerializeField] private VoidFormConfigSO voidFormConfig;
    [SerializeField] private GravityFormConfigSO gravityFormConfig;

    private Player _player;
    private PlayerInput _input;
    private Dictionary<int, IPlayerForm> _forms;
    private Dictionary<int, bool> _unlockedForms;
    private IPlayerForm _currentForm;
    private float _switchCooldownTimer;

    public IPlayerForm CurrentForm => _currentForm;
    public int CurrentFormID => _currentForm?.FormID ?? 0;

    void Awake()
    {
        _player = GetComponent<Player>();
        _input = GetComponent<PlayerInput>();
        
        // FormUnlockManager.UnlockAll();
        
        InitializeForms();
        InitializeUnlockStates();
    }

    void OnEnable()
    {
        _input.On<int>(PlayerInputType.FormSelect, OnFormSelectInput);
        _input.On<bool>(PlayerInputType.Skill, OnSkillInput);
        EventBus.On<StateChangeEventData>(PlayerActionEventType.OnStateChanged, OnPlayerStateChanged);
        EventBus.On<int>(FormEventType.OnFormUnlocked, OnFormUnlocked);
    }

    void OnDisable()
    {
        _input.Off<int>(PlayerInputType.FormSelect, OnFormSelectInput);
        _input.Off<bool>(PlayerInputType.Skill, OnSkillInput);
        EventBus.Off<StateChangeEventData>(PlayerActionEventType.OnStateChanged, OnPlayerStateChanged);
        EventBus.Off<int>(FormEventType.OnFormUnlocked, OnFormUnlocked);
    }

    void Update()
    {
        UpdateCooldown();
        _currentForm?.OnUpdate();
    }

    private void InitializeForms()
    {
        _forms = new Dictionary<int, IPlayerForm>();

        if (defaultFormConfig != null)
            _forms[defaultFormConfig.formID] = new DefaultForm(defaultFormConfig);
        
        if (agilityFormConfig != null)
            _forms[agilityFormConfig.formID] = new AgilityForm(agilityFormConfig);
        
        if (voidFormConfig != null)
            _forms[voidFormConfig.formID] = new VoidForm(voidFormConfig);
        
        if (gravityFormConfig != null)
            _forms[gravityFormConfig.formID] = new GravityForm(gravityFormConfig);

        if (_forms.TryGetValue(0, out var defaultForm))
        {
            _currentForm = defaultForm;
            _currentForm.OnEnter(_player);
        }
    }

    private void InitializeUnlockStates()
    {
        _unlockedForms = new Dictionary<int, bool>
        {
            { 0, true },
            { 1, FormUnlockManager.IsUnlocked(1) },
            { 2, FormUnlockManager.IsUnlocked(2) },
            { 3, FormUnlockManager.IsUnlocked(3) }
        };
    }

    private void OnFormSelectInput(int formID)
    {
        TryChangeForm(formID);
    }

    private void OnSkillInput(bool pressed)
    {
        if (!pressed) return;
        if (_currentForm == null || !_currentForm.HasActiveSkill) return;
        
        _currentForm.OnSkillPressed();
    }
    
    private void OnPlayerStateChanged(StateChangeEventData data)
    {
        _currentForm?.OnPlayerStateChanged(data.FromState, data.ToState);
    }

    private void OnFormUnlocked(int formID)
    {
        if (_unlockedForms.ContainsKey(formID))
        {
            _unlockedForms[formID] = true;
            Debug.Log($"[FormController] Updated unlock state for form {formID}");
        }
    }

    public void TryChangeForm(int targetFormID)
    {
        if (_switchCooldownTimer > 0f)
        {
            Debug.Log($"[FormController] Cooldown active: {_switchCooldownTimer:F1}s remaining");
            return;
        }

        if (!_unlockedForms.TryGetValue(targetFormID, out bool isUnlocked) || !isUnlocked)
        {
            Debug.Log($"[FormController] Form {targetFormID} is locked");
            return;
        }

        if (!_forms.TryGetValue(targetFormID, out var targetForm))
        {
            Debug.Log($"[FormController] Form {targetFormID} not found");
            return;
        }

        int previousFormID = CurrentFormID;
        string previousFormName = _currentForm?.FormName ?? "None";

        if (CurrentFormID == targetFormID && targetFormID != 0)
        {
            ChangeToForm(0);
            Debug.Log($"[FormController] Switched: {previousFormName} (ID:{previousFormID}) -> Default (ID:0)");
        }
        else if (CurrentFormID != targetFormID)
        {
            ChangeToForm(targetFormID);
            Debug.Log($"[FormController] Switched: {previousFormName} (ID:{previousFormID}) -> {_currentForm.FormName} (ID:{CurrentFormID})");
        }
    }

    private void ChangeToForm(int formID)
    {
        if (!_forms.TryGetValue(formID, out var newForm)) return;

        int fromID = CurrentFormID;
        string fromName = _currentForm?.FormName ?? "None";

        _currentForm?.OnExit();
        _currentForm = newForm;
        _currentForm.OnEnter(_player);

        BaseFormConfigSO config = GetConfigByFormID(fromID);
        if (config != null)
        {
            _switchCooldownTimer = config.formSwitchCooldown;
        }

        var changeData = new FormChangeData(fromID, formID, fromName, _currentForm.FormName);
        EventBus.Emit(FormEventType.OnFormChanged, changeData);

        AudioController.Instance.Play("CHANGE_FORM");
    }

    private BaseFormConfigSO GetConfigByFormID(int formID)
    {
        return formID switch
        {
            0 => defaultFormConfig,
            1 => agilityFormConfig,
            2 => voidFormConfig,
            3 => gravityFormConfig,
            _ => null
        };
    }

    private void UpdateCooldown()
    {
        if (_switchCooldownTimer > 0f)
        {
            _switchCooldownTimer -= Time.deltaTime;
            
            if (_switchCooldownTimer <= 0f)
            {
                _switchCooldownTimer = 0f;
            }
        }
    }

    public void UnlockForm(int formID)
    {
        _unlockedForms[formID] = true;
        FormUnlockManager.Unlock(formID);
        EventBus.Emit(FormEventType.OnFormUnlocked, formID);
    }

    public bool IsFormUnlocked(int formID)
    {
        return _unlockedForms.TryGetValue(formID, out bool unlocked) && unlocked;
    }

    public void RefreshUnlockStates()
    {
        _unlockedForms[1] = FormUnlockManager.IsUnlocked(1);
        _unlockedForms[2] = FormUnlockManager.IsUnlocked(2);
        _unlockedForms[3] = FormUnlockManager.IsUnlocked(3);
    }
}
