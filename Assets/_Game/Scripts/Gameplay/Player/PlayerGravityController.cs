using UnityEngine;

public class PlayerGravityController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualTransform;
    
    private Rigidbody2D _rb;
    private int _gravityDirection = 1;
    
    public int GravityDirection => _gravityDirection;
    public bool IsGravityFlipped => _gravityDirection == -1;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        if (_rb == null)
        {
            Debug.LogError("PlayerGravityController requires Rigidbody2D!", this);
        }
    }
    
    public void ToggleGravity()
    {
        if (_rb == null) return;
        
        _rb.gravityScale *= -1f;
        _gravityDirection *= -1;
        FlipVisual();
        
        string state = _gravityDirection == -1 ? "flipped" : "normal";
        Debug.Log($"[GravityController] Gravity {state} - scale: {_rb.gravityScale}");
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
