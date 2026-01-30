using UnityEngine;

/// <summary>
/// Controller quản lý UI cooldown cho các skill forms
/// Lắng nghe FormChangeData và hiển thị cooldown khi đổi form
/// </summary>
public class SkillUIController : MonoBehaviour
{
    [Header("Skill Slots - Theo thứ tự Form ID")]
    [SerializeField] private SkillSlotUI[] skillSlots = new SkillSlotUI[3];
    
    [Header("Form Configs - Để lấy cooldown duration")]
    [SerializeField] private BaseFormConfigSO defaultFormConfig;
    [SerializeField] private BaseFormConfigSO agilityFormConfig;
    [SerializeField] private BaseFormConfigSO voidFormConfig;
    [SerializeField] private BaseFormConfigSO gravityFormConfig;
    
    private PlayerFormController _formController;

    void OnEnable()
    {
        // Lắng nghe event OnFormChanged và OnFormUnlocked
        EventBus.On<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
        EventBus.On<int>(FormEventType.OnFormUnlocked, OnFormUnlocked);
    }

    void OnDisable()
    {
        // Unsubscribe events
        EventBus.Off<FormChangeData>(FormEventType.OnFormChanged, OnFormChanged);
        EventBus.Off<int>(FormEventType.OnFormUnlocked, OnFormUnlocked);
    }

    void Start()
    {
        // Tìm PlayerFormController
        _formController = FindObjectOfType<PlayerFormController>();
        
        if (_formController == null)
        {
            Debug.LogError("[SkillUI] Cannot find PlayerFormController!");
        }
        
        // Setup icons cho các skill slots
        SetupSkillIcons();
        
        // Setup locked states cho các forms
        InitializeLockedStates();
    }

    /// <summary>
    /// Xử lý khi nhận event OnFormChanged
    /// </summary>
    private void OnFormChanged(FormChangeData data)
    {
        // Lấy config của form vừa rời khỏi
        BaseFormConfigSO fromConfig = GetFormConfig(data.FromFormID);
        
        if (fromConfig == null) return;
        
        // Lấy cooldown duration từ config
        float cooldownDuration = fromConfig.formSwitchCooldown;
        
        // Trigger cooldown cho TẤT CẢ skills đã unlock, NGOẠI TRỪ form hiện tại (ToFormID)
        StartCooldownForAllUnlockedSkills(cooldownDuration, data.ToFormID);
        
        // Update icon của slot 0 (Default) để hiển thị form hiện tại
        UpdateDefaultSlotIcon(data.ToFormID);
        
        Debug.Log($"[SkillUI] Started cooldown for ALL unlocked skills (except current form {data.ToFormID}) - {cooldownDuration}s");
    }
    
    /// <summary>
    /// Xử lý khi form được unlock
    /// </summary>
    private void OnFormUnlocked(int formID)
    {
        if (formID >= 0 && formID < skillSlots.Length)
        {
            SkillSlotUI slot = skillSlots[formID];
            if (slot != null)
            {
                slot.SetLocked(false);
                Debug.Log($"[SkillUI] Form {formID} unlocked!");
            }
        }
    }

    /// <summary>
    /// Lấy form config theo ID
    /// </summary>
    private BaseFormConfigSO GetFormConfig(int formID)
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

    /// <summary>
    /// Setup icons cho các skill slots
    /// </summary>
    private void SetupSkillIcons()
    {
        SetSkillIcon(0, defaultFormConfig);
        SetSkillIcon(1, agilityFormConfig);
        SetSkillIcon(2, voidFormConfig);
        SetSkillIcon(3, gravityFormConfig);
    }
    
    /// <summary>
    /// Khởi tạo trạng thái locked cho các skills
    /// </summary>
    private void InitializeLockedStates()
    {
        if (_formController == null) return;
        
        for (int i = 0; i < skillSlots.Length; i++)
        {
            if (skillSlots[i] != null)
            {
                bool isLocked = !_formController.IsFormUnlocked(i);
                skillSlots[i].SetLocked(isLocked);
            }
        }
    }
    
    /// <summary>
    /// Start cooldown cho tất cả skills đã unlock, ngoại trừ form được chỉ định
    /// </summary>
    private void StartCooldownForAllUnlockedSkills(float duration, int excludeFormID = -1)
    {
        for (int i = 0; i < skillSlots.Length; i++)
        {
            var slot = skillSlots[i];
            
            // Skip nếu slot null, locked, hoặc là form hiện tại (excludeFormID)
            if (slot == null || slot.IsLocked || i == excludeFormID)
                continue;
            
            slot.StartCooldown(duration);
        }
    }

    /// <summary>
    /// Set icon cho một skill slot
    /// </summary>
    private void SetSkillIcon(int slotIndex, BaseFormConfigSO config)
    {
        if (slotIndex >= 0 && slotIndex < skillSlots.Length)
        {
            SkillSlotUI slot = skillSlots[slotIndex];
            if (slot != null && config != null && config.icon != null)
            {
                slot.SetIcon(config.icon);
            }
        }
    }

    /// <summary>
    /// Update icon của slot 0 (Default) để hiển thị form hiện tại
    /// </summary>
    private void UpdateDefaultSlotIcon(int currentFormID)
    {
        if (skillSlots[0] == null) return;
        
        BaseFormConfigSO currentConfig = GetFormConfig(currentFormID);
        
        if (currentConfig != null && currentConfig.icon != null)
        {
            skillSlots[0].SetIcon(currentConfig.icon);
        }
    }
    
    /// <summary>
    /// Reset tất cả cooldowns (dùng khi restart level, etc.)
    /// </summary>
    public void ResetAllCooldowns()
    {
        foreach (var slot in skillSlots)
        {
            slot?.ResetCooldown();
        }
    }
}
