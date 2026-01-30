using UnityEngine;
using UnityEngine.UI;

public class DialogueTesterButton : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueData testDialogueData;
    [SerializeField] private DialogueText legacyTestDialogue;
    [SerializeField] private Button testButton;
    
    void Start()
    {
        if (dialogueController == null || testButton == null) return;
        if (testDialogueData == null && legacyTestDialogue == null) return;
        
        testButton.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
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
}
