using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : DialogueBase
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private DialogueText legacyDialogueText;
    
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool disablePlayerMovement = true;
    
    private bool hasTriggered = false;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered)
            {
                return;
            }
            
            TriggerDialogue(other.gameObject);
        }
    }
    
    private void TriggerDialogue(GameObject player)
    {
        var controller = GetDialogueController();
        if (controller == null) return;
        
        if (dialogueData == null && legacyDialogueText == null)
        {
            return;
        }
        
        if (disablePlayerMovement)
        {
            PlayerMovementController.Disable(player);
        }
        
        if (dialogueData != null)
        {
            ShowDialogue(dialogueData);
        }
        else
        {
            ShowDialogue(legacyDialogueText);
        }
        
        hasTriggered = true;
        
        if (triggerOnce)
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }
    
    public void OnDialogueEnd(GameObject player)
    {
        if (disablePlayerMovement && player != null)
        {
            PlayerMovementController.Enable(player);
        }
    }
    
    // Visualize trigger zone trong Editor
    void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f); // Green semi-transparent
            
            if (col is BoxCollider2D box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.offset, box.size);
            }
            else if (col is CircleCollider2D circle)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            
            if (col is BoxCollider2D box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(box.offset, box.size);
            }
            else if (col is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
            }
        }
    }
}
