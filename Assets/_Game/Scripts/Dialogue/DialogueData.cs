using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/New Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Dialogue Configuration")]
    public DialogueLine[] lines;
    
    [Header("Optional Settings")]
    [Tooltip("Tự động đóng dialogue sau dòng cuối")]
    public bool autoClose = true;
    
    [Tooltip("Thời gian delay trước khi auto close (nếu bật)")]
    public float autoCloseDelay = 0.5f;
}
