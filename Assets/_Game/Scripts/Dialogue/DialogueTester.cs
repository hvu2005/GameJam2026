using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText testDialogue;
    
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
        if (dialogueController != null && testDialogue != null)
        {
            dialogueController.DisplayNextParagraph(testDialogue);
        }
    }
    
    public void OnButtonClick()
    {
        TriggerDialogue();
    }
}
