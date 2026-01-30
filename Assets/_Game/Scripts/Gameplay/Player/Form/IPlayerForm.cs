public interface IPlayerForm
{
    int FormID { get; }
    string FormName { get; }
    bool HasActiveSkill { get; }
    int GetMaxJumps();
    void OnEnter(Player player);
    void OnUpdate();
    void OnExit();
    void OnSkillPressed();
}
