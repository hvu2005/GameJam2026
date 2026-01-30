using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class NPC : DialogueBase, IInteractable, ITalkable
{
    [SerializeField] protected DialogueData dialogueData;
    [SerializeField] protected DialogueText legacyDialogueText;
    
    void Awake()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
    
    public virtual void Interact()
    {
        if (dialogueData != null)
        {
            Talk(dialogueData);
        }
        else if (legacyDialogueText != null)
        {
            Talk(legacyDialogueText);
        }
    }
    
    public void Talk(DialogueData data)
    {
        ShowDialogue(data);
    }
    
    public void Talk(DialogueText text)
    {
        ShowDialogue(text);
    }
}
