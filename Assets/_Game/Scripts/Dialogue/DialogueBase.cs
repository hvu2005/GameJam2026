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
    
    protected void ShowDialogue(DialogueData dialogueData, GameObject objToDestroy = null, bool unlockSkill = false, int skillId = 0, GameObject tutObject = null)
    {
        var controller = GetDialogueController();
        if (controller != null && dialogueData != null)
        {
            controller.StartDialogue(dialogueData, objToDestroy, unlockSkill, skillId, tutObject);
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
