using UnityEngine;

public interface ITalkable
{
    void Talk(DialogueData dialogueData);
    void Talk(DialogueText dialogueText);
}
