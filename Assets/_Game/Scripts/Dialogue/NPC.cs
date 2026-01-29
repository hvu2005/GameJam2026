using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class NPC : DialogueBase, IInteractable, ITalkable
{
    [SerializeField] protected DialogueText dialogueText;
    
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
        Talk(dialogueText);
    }
    
    public void Talk(DialogueText text)
    {
        ShowDialogue(text);
    }
}
