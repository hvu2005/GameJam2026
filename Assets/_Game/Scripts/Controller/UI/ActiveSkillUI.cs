using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// UI hiển thị cooldown của skill hiện tại (Void Teleport, Gravity Flip, etc.)
/// Lắng nghe event OnFormSkillCooldownStart để bắt đầu hiển thị cooldown
/// </summary>
public class ActiveSkillUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private GameObject skillContainer;
    [SerializeField] private GameObject lockedOverlay;
    
    [Header("Settings")]
    [SerializeField] private float fillSpeed = 5f;
    
    private Input _inputActions;
    private float _cooldownTimer = 0f;
    private float _cooldownDuration = 0f;
    private bool _isOnCooldown = false;
    private bool _isInFillPhase = false;
    private int _currentFormID = -1;
    
    void OnEnable()
    {
        // Lắng nghe skill cooldown event
        EventBus.On<float>(FormEventType.OnFormSkillCooldownStart, OnSkillCooldownStart);
        
        // Lắng nghe form change để update icon
        EventBus.On<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
        
        // Tạo Input Actions để lấy binding
        if (_inputActions == null)
        {
            _inputActions = new Input();
        }
        
        // Lắng nghe rebinding events
        InputSystem.onActionChange += OnActionChange;
    }
    
    void OnDisable()
    {
        EventBus.Off<float>(FormEventType.OnFormSkillCooldownStart, OnSkillCooldownStart);
        EventBus.Off<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
        
        InputSystem.onActionChange -= OnActionChange;
    }
    
    void Start()
    {
        // Update skill key text từ Input System
        UpdateSkillKeyText();
        
        // Ẩn cooldown UI ban đầu
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 0f;
            cooldownFill.enabled = false;
        }
        
        if (cooldownText != null)
        {
            cooldownText.enabled = false;
        }
    }
    
    void Update()
    {
        if (_isInFillPhase)
        {
            UpdateFillAnimation();
        }
        else if (_isOnCooldown)
        {
            _cooldownTimer -= Time.deltaTime;
            
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                _isOnCooldown = false;
                StartFillPhase();
            }
            
            UpdateCooldownText();
        }
    }
    
    private void OnFormChanged(FormChangeData data)
    {
        _currentFormID = data.ToFormID;
        
        // Chỉ hiển thị skill UI cho form có active skill (Void = 2, Gravity = 3)
        bool hasActiveSkill = data.ToFormID == 2 || data.ToFormID == 3;
        
        if (skillContainer != null)
        {
            skillContainer.SetActive(true); // Luôn hiện container
        }
        
        // Hiện/ẩn locked overlay
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!hasActiveSkill);
        }
        
        // Chỉ reset cooldown nếu đang hiển thị
        if (hasActiveSkill)
        {
            ResetCooldown();
        }
    }
    
    private void OnSkillCooldownStart(float duration)
    {
        StartCooldown(duration);
    }
    
    private void StartCooldown(float duration)
    {
        _cooldownDuration = duration;
        _cooldownTimer = duration;
        _isOnCooldown = true;
        _isInFillPhase = false;
        
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f;
            cooldownFill.enabled = true;
        }
        
        if (cooldownText != null)
        {
            cooldownText.enabled = true;
        }
    }
    
    private void StartFillPhase()
    {
        _isInFillPhase = true;
        
        if (cooldownText != null)
        {
            cooldownText.enabled = false;
        }
        
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f;
        }
    }
    
    private void UpdateFillAnimation()
    {
        if (cooldownFill == null) return;
        
        cooldownFill.fillAmount -= Time.deltaTime * fillSpeed;
        
        if (cooldownFill.fillAmount <= 0f)
        {
            cooldownFill.fillAmount = 0f;
            cooldownFill.enabled = false;
            _isInFillPhase = false;
        }
    }
    
    private void UpdateCooldownText()
    {
        if (cooldownText == null) return;
        
        int secondsLeft = Mathf.CeilToInt(_cooldownTimer);
        cooldownText.text = secondsLeft.ToString();
    }
    
    /// <summary>
    /// Update skill key text từ Input System binding
    /// </summary>
    private void UpdateSkillKeyText()
    {
        if (skillText == null || _inputActions == null) return;
        
        // Lấy binding hiện tại của Skill action
        InputAction skillAction = _inputActions.GamePlay.Skill;
        
        if (skillAction != null && skillAction.bindings.Count > 0)
        {
            // Lấy binding đầu tiên (thường là primary binding)
            string bindingPath = skillAction.bindings[0].effectivePath;
            
            // Lấy display string (tên nút dễ đọc)
            string displayString = InputControlPath.ToHumanReadableString(
                bindingPath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            
            // Viết tắt nếu dài hơn 2 ký tự
            if (displayString.Length > 2)
            {
                displayString = GetAbbreviation(displayString);
            }
            
            skillText.text = displayString;
        }
        else
        {
            skillText.text = "Sk";
        }
    }
    
    /// <summary>
    /// Lấy chữ viết tắt từ tên phím (lấy 2 chữ cái đầu)
    /// </summary>
    private string GetAbbreviation(string keyName)
    {
        if (string.IsNullOrEmpty(keyName) || keyName.Length <= 2)
            return keyName;
        
        // Lấy 2 chữ cái đầu và viết hoa
        return keyName.Substring(0, 2).ToUpper();
    }
    
    /// <summary>
    /// Callback khi Input System binding thay đổi
    /// </summary>
    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.BoundControlsChanged || 
            change == InputActionChange.ActionMapEnabled ||
            change == InputActionChange.ActionMapDisabled)
        {
            UpdateSkillKeyText();
        }
    }
    
    private void ResetCooldown()
    {
        _isOnCooldown = false;
        _isInFillPhase = false;
        _cooldownTimer = 0f;
        
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 0f;
            cooldownFill.enabled = false;
        }
        
        if (cooldownText != null)
        {
            cooldownText.enabled = false;
        }
    }
}
