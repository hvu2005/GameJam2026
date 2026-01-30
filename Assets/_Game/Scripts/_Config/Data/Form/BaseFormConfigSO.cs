using UnityEngine;

public abstract class BaseFormConfigSO : ScriptableObject
{
    [Header("Form Info")]
    public int formID;
    public string formName;
    public Sprite icon;
    
    [Header("Cooldown")]
    [Tooltip("Cooldown time when switching away from this form")]
    public float formSwitchCooldown = 2f;

    public abstract bool HasActiveSkill { get; }
    public abstract int GetMaxJumps();
}
