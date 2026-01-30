using UnityEngine;

public enum SpeakerPosition
{
    Left,
    Right,
    Center
}

[System.Serializable]
public class DialogueLine
{
    [Header("Speaker Info")]
    public string speakerName;
    public Sprite portrait;
    public SpeakerPosition position = SpeakerPosition.Left;
    
    [Header("Dialogue")]
    [TextArea(3, 10)]
    public string text;
}
