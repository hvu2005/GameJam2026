using UnityEngine;

/// <summary>
/// Script để player tương tác với NPC trong game
/// Attach vào Player GameObject
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("UI Prompt")]
    [SerializeField] private GameObject interactionPrompt; // UI "Press E to talk"
    
    private IInteractable currentInteractable;
    private bool isInDialogue = false;
    
    void Update()
    {
        if (isInDialogue)
        {
            // Đang trong dialogue, không check interaction
            return;
        }
        
        // Tìm NPC gần nhất
        CheckForInteractable();
        
        // Nhấn E để tương tác (dùng UnityEngine.Input)
        if (currentInteractable != null && UnityEngine.Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
            isInDialogue = true;
        }
    }
    
    private void CheckForInteractable()
    {
        // Tìm tất cả Collider2D trong range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayer);
        
        IInteractable nearestInteractable = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestInteractable = interactable;
                }
            }
        }
        
        // Update current interactable
        if (nearestInteractable != currentInteractable)
        {
            currentInteractable = nearestInteractable;
            UpdatePrompt();
        }
    }
    
    private void UpdatePrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(currentInteractable != null);
        }
    }
    
    // Gọi khi dialogue kết thúc
    public void OnDialogueEnd()
    {
        isInDialogue = false;
        currentInteractable = null;
        UpdatePrompt();
    }
    
    // Debug visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
