using UnityEngine;
using System.Collections;

public class PlayerGravityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualTransform;
    
    private Rigidbody2D _rb;
    private int _gravityDirection = 1;
    private float _baseGravityScale;
    private float _reverseGravityMultiplier = 1f;
    private float _reverseGravityBoostDuration = 0f;
    private Coroutine _gravityBoostCoroutine;
    private bool _isGravityBoosted = false;
    
    public int GravityDirection => _gravityDirection;
    public bool IsGravityFlipped => _gravityDirection == -1;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        if (_rb == null)
        {
            Debug.LogError("PlayerGravityController requires Rigidbody2D!", this);
        }
        else
        {
            _baseGravityScale = Mathf.Abs(_rb.gravityScale);
        }
    }
    
    public void SetReverseGravityConfig(float multiplier, float duration)
    {
        _reverseGravityMultiplier = multiplier;
        _reverseGravityBoostDuration = duration;
    }
    
    public void ToggleGravity()
    {
        if (_rb == null) return;
        
        // Đảo direction
        _gravityDirection *= -1;
        
        // Dừng coroutine cũ nếu đang chạy
        if (_gravityBoostCoroutine != null)
        {
            StopCoroutine(_gravityBoostCoroutine);
        }
        
        // Apply gravity với boost tạm thời
        if (_reverseGravityMultiplier > 1f && _reverseGravityBoostDuration > 0f)
        {
            // Tăng gravity tạm thời
            _rb.gravityScale = _baseGravityScale * _gravityDirection * _reverseGravityMultiplier;
            _isGravityBoosted = true;
            _gravityBoostCoroutine = StartCoroutine(ResetGravityAfterDelay());
        }
        else
        {
            // Không có boost, chỉ đảo bình thường
            _rb.gravityScale = _baseGravityScale * _gravityDirection;
            _isGravityBoosted = false;
        }
        
        FlipVisual();
        
        string state = _gravityDirection == -1 ? "flipped" : "normal";
        Debug.Log($"[GravityController] Gravity {state} - scale: {_rb.gravityScale} (boost: {_reverseGravityMultiplier}x for {_reverseGravityBoostDuration}s)");
    }
    
    private IEnumerator ResetGravityAfterDelay()
    {
        yield return new WaitForSeconds(_reverseGravityBoostDuration);
        
        // Fallback: Reset về gravity bình thường nếu chưa chạm ground
        if (_isGravityBoosted)
        {
            ResetGravityBoost("timeout");
        }
    }
    
    private void ResetGravityBoost(string reason)
    {
        if (!_isGravityBoosted) return;
        
        // Reset về gravity bình thường (giữ nguyên direction)
        _rb.gravityScale = _baseGravityScale * _gravityDirection;
        _isGravityBoosted = false;
        Debug.Log($"[GravityController] Gravity boost ended ({reason}) - scale reset to: {_rb.gravityScale}");
    }
    
    // Detect collision để reset gravity boost
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isGravityBoosted) return;
        
        // Check nếu collision theo hướng gravity (chạm ground)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Nếu đang flip (bay lên), check chạm trần (normal hướng xuống)
            // Nếu đang normal (rơi xuống), check chạm đất (normal hướng lên)
            float dotProduct = Vector2.Dot(contact.normal, Vector2.up);
            
            if (_gravityDirection == -1 && dotProduct < -0.5f) // Flip: chạm trần
            {
                ResetGravityBoost("collision");
                break;
            }
            else if (_gravityDirection == 1 && dotProduct > 0.5f) // Normal: chạm đất
            {
                ResetGravityBoost("collision");
                break;
            }
        }
    }
    
    public float GetCurrentGravityScale(float baseGravityScale)
    {
        return Mathf.Abs(baseGravityScale) * _gravityDirection;
    }
    
    private void FlipVisual()
    {
        if (visualTransform == null)
        {
            Debug.LogWarning("Visual transform not assigned to PlayerGravityController!");
            return;
        }
        
        Vector3 scale = visualTransform.localScale;
        scale.y = Mathf.Abs(scale.y) * _gravityDirection;
        visualTransform.localScale = scale;
    }
}
