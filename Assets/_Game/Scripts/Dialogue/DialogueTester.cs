using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueData testDialogueData;
    [SerializeField] private DialogueText legacyTestDialogue;
    
    void Update()
    {
        bool spacePressed = false;
        
        try
        {
            spacePressed = UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space);
        }
        catch (System.Exception)
        {
        }
        
        #if ENABLE_INPUT_SYSTEM
        if (!spacePressed)
        {
            try
            {
                var keyboard = UnityEngine.InputSystem.Keyboard.current;
                if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
                {
                    spacePressed = true;
                }
            }
            catch (System.Exception)
            {
            }
        }
        #endif
        
        if (spacePressed)
        {
            TriggerDialogue();
        }
    }
    
    private void TriggerDialogue()
    {
        if (dialogueController != null)
        {
            if (testDialogueData != null)
            {
                dialogueController.StartDialogue(testDialogueData);
            }
            else if (legacyTestDialogue != null)
            {
                dialogueController.DisplayNextParagraph(legacyTestDialogue);
            }
        }
    }
    
    public void OnButtonClick()
    {
        TriggerDialogue();
    }
}
