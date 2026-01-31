using UnityEngine;

public abstract class DialogueBase : MonoBehaviour
{
    protected DialogueController dialogueController;
    
    protected DialogueController GetDialogueController()
    {
        if (dialogueController == null)
        {
            dialogueController = FindObjectOfType<DialogueController>(true);
        }
        return dialogueController;
    }
    
    protected void ShowDialogue(DialogueData dialogueData, GameObject objToDestroy = null)
    {
        var controller = GetDialogueController();
        if (controller != null && dialogueData != null)
        {
            controller.StartDialogue(dialogueData, objToDestroy);
        }
    }
    
    // Backwards compatibility
    protected void ShowDialogue(DialogueText dialogueText)
    {
        var controller = GetDialogueController();
        if (controller != null && dialogueText != null)
        {
            controller.DisplayNextParagraph(dialogueText);
        }
    }
}
