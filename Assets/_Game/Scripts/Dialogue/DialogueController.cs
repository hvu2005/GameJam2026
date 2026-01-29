using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private float typeSpeed = 10f;
    
    private Queue<string> paragraphs = new Queue<string>();
    private bool conversationEnded;
    private string p;
    private bool isTyping;
    private Coroutine typingDialogueCoroutine;
    
    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_SPEED = 0.1f;
    
    private DialogueText currentDialogueText;
    private GameObject dialoguePanel;
    
    void Awake()
    {
        // Tìm DialoguePanel - có thể là parent hoặc chính object này
        if (transform.parent != null && transform.parent.name.Contains("Panel"))
        {
            dialoguePanel = transform.parent.gameObject;
        }
        else
        {
            dialoguePanel = gameObject;
        }
        
        // Đảm bảo panel inactive lúc start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Chỉ check input khi panel đang active
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;
        
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            DisplayNextParagraph(currentDialogueText);
        }
    }
    
    public void DisplayNextParagraph(DialogueText dialogueText)
    {
        if (dialogueText != null)
        {
            currentDialogueText = dialogueText;
        }
        
        // Bật panel nếu chưa active
        if (dialoguePanel != null && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }
        
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                EndConversation();
                return;
            }
        }
        
        if (!isTyping)
        {
            p = paragraphs.Dequeue();
            typingDialogueCoroutine = StartCoroutine(TypeDialogueText(p));
        }
        else
        {
            FinishParagraphEarly();
        }
        
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
        }
    }
    
    private void StartConversation(DialogueText dialogueText)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }
        
        NPCNameText.text = dialogueText.speakerName;
        
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
        }
    }
    
    private void EndConversation()
    {
        paragraphs.Clear();
        conversationEnded = false;
        
        // Tắt panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovementController.Enable(player);
        }
        
        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.OnDialogueEnd();
        }
    }
    
    private IEnumerator TypeDialogueText(string p)
    {
        isTyping = true;
        NPCDialogueText.text = "";
        
        string originalText = p;
        string displayedText = "";
        int alphaIndex = 0;
        
        foreach (char c in p.ToCharArray())
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;
            displayedText = NPCDialogueText.text.Insert(alphaIndex, HTML_ALPHA);
            NPCDialogueText.text = displayedText;
            
            yield return new WaitForSeconds(MAX_TYPE_SPEED / typeSpeed);
        }
        
        isTyping = false;
    }
    
    private void FinishParagraphEarly()
    {
        StopCoroutine(typingDialogueCoroutine);
        NPCDialogueText.text = p;
        isTyping = false;
    }
}
