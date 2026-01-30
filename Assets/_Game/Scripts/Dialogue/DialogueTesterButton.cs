using UnityEngine;
using UnityEngine.UI;

public class DialogueTesterButton : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private DialogueText testDialogue;
    [SerializeField] private Button testButton;
    
    void Start()
    {
        if (dialogueController == null || testDialogue == null || testButton == null)
        {
            return;
        }
        
        testButton.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        if (dialogueController != null && testDialogue != null)
        {
            dialogueController.DisplayNextParagraph(testDialogue);
        }
    }
}
