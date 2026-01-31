using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("Left Speaker Panel")]
    [SerializeField] private GameObject leftSpeakerPanel;
    [SerializeField] private Image leftPortrait;
    [SerializeField] private TextMeshProUGUI leftNameText;
    [SerializeField] private TextMeshProUGUI leftDialogueText;

    [Header("Right Speaker Panel")]
    [SerializeField] private GameObject rightSpeakerPanel;
    [SerializeField] private Image rightPortrait;
    [SerializeField] private TextMeshProUGUI rightNameText;
    [SerializeField] private TextMeshProUGUI rightDialogueText;

    [Header("Center Panel (Optional)")]
    [SerializeField] private GameObject centerPanel;
    [SerializeField] private TextMeshProUGUI centerNameText;
    [SerializeField] private TextMeshProUGUI centerDialogueText;

    [Header("Settings")]
    [SerializeField] private float typeSpeed = 10f;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private Queue<DialogueLine> dialogueLines = new Queue<DialogueLine>();
    private bool conversationEnded;
    private DialogueLine currentLine;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private bool isClosing; // Flag to prevent input during closing

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_SPEED = 0.1f;

    private DialogueData currentDialogueData;
    private GameObject dialoguePanel;
    private GameObject objectToDestroy; // Reference to the object to destroy after conversation
    private bool shouldUnlockSkill;
    private int skillIdToUnlock;
    private GameObject tutorialObject;

    void Awake()
    {
        // Tìm DialoguePanel
        // DialogueController PHẢI nằm trên object active, không phải DialoguePanel
        if (transform.parent != null && transform.parent.name.Contains("Panel"))
        {
            dialoguePanel = transform.parent.gameObject;
        }
        else
        {
            // Tìm DialoguePanel trong children hoặc parent
            Transform panelTransform = transform.Find("DialoguePanel");
            if (panelTransform == null)
            {
                panelTransform = transform.parent?.Find("DialoguePanel");
            }

            if (panelTransform != null)
            {
                dialoguePanel = panelTransform.gameObject;
            }
            else
            {
                dialoguePanel = gameObject;
            }
        }

        // Đảm bảo panel inactive lúc start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    void Update()
    {
        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;
        if (isClosing) return; // Block input during closing

        // Press Enter OR Left Mouse Click to advance dialogue
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Return) ||
            UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.KeypadEnter) ||
            UnityEngine.Input.GetMouseButtonDown(0)) // 0 = Left Click
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(DialogueData dialogueData, GameObject objToDestroy = null, bool unlockSkill = false, int skillId = 0, GameObject tutObject = null)
    {
        if (dialogueData == null || dialogueData.lines.Length == 0) return;

        currentDialogueData = dialogueData;
        objectToDestroy = objToDestroy;
        shouldUnlockSkill = unlockSkill;
        skillIdToUnlock = skillId;
        tutorialObject = tutObject;
        
        dialogueLines.Clear();
        conversationEnded = false;
        isClosing = false; // Reset closing flag

        foreach (var line in dialogueData.lines)
        {
            dialogueLines.Enqueue(line);
        }

        if (dialoguePanel != null && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (isTyping)
        {
            FinishLineEarly();
            return;
        }

        if (dialogueLines.Count == 0)
        {
            if (!conversationEnded)
            {
                conversationEnded = true;
                if (currentDialogueData != null && currentDialogueData.autoClose)
                {
                    StartCoroutine(AutoCloseDialogue());
                }
            }
            else
            {
                EndConversation();
            }
            return;
        }

        currentLine = dialogueLines.Dequeue();
        ShowLine(currentLine);
    }

    private void ShowLine(DialogueLine line)
    {
        DeactivateAllPanels();

        switch (line.position)
        {
            case SpeakerPosition.Left:
                SetupSpeakerPanel(leftSpeakerPanel, leftPortrait, leftNameText, leftDialogueText, line, true);
                SetSpeakerActive(rightSpeakerPanel, rightPortrait, false);
                break;

            case SpeakerPosition.Right:
                SetupSpeakerPanel(rightSpeakerPanel, rightPortrait, rightNameText, rightDialogueText, line, true);
                SetSpeakerActive(leftSpeakerPanel, leftPortrait, false);
                break;

            case SpeakerPosition.Center:
                if (centerPanel != null)
                {
                    centerPanel.SetActive(true);
                    centerNameText.text = line.speakerName;
                    typingCoroutine = StartCoroutine(TypeText(centerDialogueText, line.text));
                }
                else
                {
                    SetupSpeakerPanel(leftSpeakerPanel, leftPortrait, leftNameText, leftDialogueText, line, true);
                }
                break;
        }
    }

    private void SetupSpeakerPanel(GameObject panel, Image portrait, TextMeshProUGUI nameText, TextMeshProUGUI dialogueText, DialogueLine line, bool isActive)
    {
        if (panel == null) return;

        panel.SetActive(true);
        nameText.text = line.speakerName;

        if (portrait != null && line.portrait != null)
        {
            portrait.sprite = line.portrait;
            portrait.gameObject.SetActive(true);
            portrait.color = isActive ? activeColor : inactiveColor;
        }
        else if (portrait != null)
        {
            portrait.gameObject.SetActive(false);
        }

        typingCoroutine = StartCoroutine(TypeText(dialogueText, line.text));
    }

    private void SetSpeakerActive(GameObject panel, Image portrait, bool isActive)
    {
        if (panel != null && panel.activeSelf && portrait != null)
        {
            portrait.color = isActive ? activeColor : inactiveColor;
        }
    }

    private void DeactivateAllPanels()
    {
        if (leftSpeakerPanel != null) leftSpeakerPanel.SetActive(false);
        if (rightSpeakerPanel != null) rightSpeakerPanel.SetActive(false);
        if (centerPanel != null) centerPanel.SetActive(false);
    }

    private IEnumerator TypeText(TextMeshProUGUI textComponent, string text)
    {
        isTyping = true;
        textComponent.text = "";

        string originalText = text;
        string displayedText = "";
        int alphaIndex = 0;

        foreach (char c in text.ToCharArray())
        {
            alphaIndex++;
            textComponent.text = originalText;
            displayedText = textComponent.text.Insert(alphaIndex, HTML_ALPHA);
            textComponent.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_SPEED / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishLineEarly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (currentLine != null)
        {
            switch (currentLine.position)
            {
                case SpeakerPosition.Left:
                    if (leftDialogueText != null) leftDialogueText.text = currentLine.text;
                    break;
                case SpeakerPosition.Right:
                    if (rightDialogueText != null) rightDialogueText.text = currentLine.text;
                    break;
                case SpeakerPosition.Center:
                    if (centerDialogueText != null) centerDialogueText.text = currentLine.text;
                    break;
            }
        }

        isTyping = false;
    }

    private IEnumerator AutoCloseDialogue()
    {
        yield return new WaitForSeconds(currentDialogueData.autoCloseDelay);
        EndConversation();
    }

    private void EndConversation()
    {
        isClosing = true; // Block all input immediately

        dialogueLines.Clear();
        conversationEnded = false;
        currentDialogueData = null;

        DeactivateAllPanels();

        // Re-enable player BEFORE deactivating panel (to avoid disabling this MonoBehaviour)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerInteraction playerInteraction = FindObjectOfType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.OnDialogueEnd();
        }

        // Destroy optional object if set
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
            objectToDestroy = null;
        }

        // Handle Skill Unlock
        if (shouldUnlockSkill)
        {
            FormUnlockManager.Unlock(skillIdToUnlock);
            EventBus.Emit(FormEventType.OnFormUnlocked, skillIdToUnlock);
            
            if (tutorialObject != null)
            {
                // Enable the existing tutorial object
                tutorialObject.SetActive(true);
            }
        }

        // Deactivate panel LAST
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Wait 3 frames before re-enabling to ensure Enter is fully released
        if (player != null)
        {
            Invoke(nameof(ReEnablePlayer), 0.05f); // 50ms delay (~3 frames at 60fps)
        }
    }

    private void ReEnablePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerMovementController.Enable(player);
        }
        isClosing = false; // Reset flag after re-enable
    }

    // Backwards compatibility - convert old DialogueText to new DialogueData
    public void DisplayNextParagraph(DialogueText oldDialogueText)
    {
        if (oldDialogueText == null) return;

        DialogueData tempData = ScriptableObject.CreateInstance<DialogueData>();
        tempData.lines = new DialogueLine[oldDialogueText.paragraphs.Length];

        for (int i = 0; i < oldDialogueText.paragraphs.Length; i++)
        {
            tempData.lines[i] = new DialogueLine
            {
                speakerName = oldDialogueText.speakerName,
                text = oldDialogueText.paragraphs[i],
                position = SpeakerPosition.Left
            };
        }

        StartDialogue(tempData);
    }
}
