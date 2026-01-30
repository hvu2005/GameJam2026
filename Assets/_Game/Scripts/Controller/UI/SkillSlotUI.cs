using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Component quản lý UI của một skill slot với cooldown fill effect
/// </summary>
public class SkillSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    
    [Header("Settings")]
    [SerializeField] private float fillSpeed = 5f; // Tốc độ fill (càng cao càng nhanh)
    
    private float _cooldownTimer = 0f;
    private float _cooldownDuration = 0f;
    private bool _isOnCooldown = false;
    private bool _isInFillPhase = false; // Phase fill sau khi countdown xong
    private bool _isLocked = true;

    void Update()
    {
        if (_isInFillPhase)
        {
            // Phase 2: Fill animation nhanh
            UpdateFillAnimation();
        }
        else if (_isOnCooldown)
        {
            // Phase 1: Text countdown
            _cooldownTimer -= Time.deltaTime;
            
            if (_cooldownTimer <= 0f)
            {
                _cooldownTimer = 0f;
                _isOnCooldown = false;
                
                // Chuyển sang phase fill
                StartFillPhase();
            }
            
            UpdateCooldownText();
        }
    }

    /// <summary>
    /// Bắt đầu cooldown fill effect
    /// </summary>
    public void StartCooldown(float duration)
    {
        // Không cooldown nếu skill đang locked
        if (_isLocked) return;
        
        _cooldownDuration = duration;
        _cooldownTimer = duration;
        _isOnCooldown = true;
        _isInFillPhase = false;
        
        // Hiện fill ở 100% để ẩn icon trong phase countdown
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f; // Full để ẩn icon
            cooldownFill.enabled = true; // Hiện ngay
        }
        
        if (cooldownText != null)
        {
            cooldownText.enabled = true;
        }
    }
    
    /// <summary>
    /// Bắt đầu phase fill animation
    /// </summary>
    private void StartFillPhase()
    {
        _isInFillPhase = true;
        
        // Ẩn text, giữ fill (đã enabled từ phase 1)
        if (cooldownText != null)
        {
            cooldownText.enabled = false;
        }
        
        // Fill đã enabled từ phase countdown, chỉ cần đảm bảo ở 100%
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = 1f; // Bắt đầu từ 100%
            // cooldownFill.enabled đã = true từ StartCooldown
        }
    }

    /// <summary>
    /// Update fill animation (siêu nhanh từ 100% -> 0%)
    /// </summary>
    private void UpdateFillAnimation()
    {
        if (cooldownFill == null) return;
        
        // Giảm fill amount nhanh
        cooldownFill.fillAmount -= fillSpeed * Time.deltaTime;
        
        // Hoàn thành khi fill về 0
        if (cooldownFill.fillAmount <= 0f)
        {
            cooldownFill.fillAmount = 0f;
            cooldownFill.enabled = false;
            _isInFillPhase = false;
        }
    }
    
    /// <summary>
    /// Update fill amount dựa trên timer (KHÔNG DÙNG NỮA - giữ lại để tương thích)
    /// </summary>
    private void UpdateFillAmount()
    {
        // Method này không còn được gọi - fill giờ chạy trong UpdateFillAnimation
    }
    
    /// <summary>
    /// Update text hiển thị số giây còn lại
    /// </summary>
    private void UpdateCooldownText()
    {
        if (cooldownText == null) return;
        
        if (_cooldownTimer > 0f)
        {
            // Hiển thị số giây (làm tròn lên)
            int seconds = Mathf.CeilToInt(_cooldownTimer);
            cooldownText.text = seconds.ToString();
            cooldownText.enabled = true;
        }
        else
        {
            cooldownText.enabled = false;
        }
    }

    /// <summary>
    /// Set icon cho skill
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        if (skillIcon != null && icon != null)
        {
            skillIcon.sprite = icon;
        }
    }

    /// <summary>
    /// Reset cooldown ngay lập tức
    /// </summary>
    public void ResetCooldown()
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

    /// <summary>
    /// Set trạng thái locked/unlocked cho skill
    /// </summary>
    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        
        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(locked);
        }
        
        // Reset cooldown nếu đang locked
        if (locked)
        {
            ResetCooldown();
        }
        else
        {
            // Ẩn fill và text khi unlock (trạng thái ban đầu)
            if (cooldownFill != null)
            {
                cooldownFill.enabled = false;
                cooldownFill.fillAmount = 0f;
            }
            
            if (cooldownText != null)
            {
                cooldownText.enabled = false;
            }
        }
    }

    /// <summary>
    /// Kiểm tra skill có đang locked không
    /// </summary>
    public bool IsLocked => _isLocked;
}
