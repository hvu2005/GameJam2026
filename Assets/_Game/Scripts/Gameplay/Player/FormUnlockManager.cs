using UnityEngine;

public static class FormUnlockManager
{
    private const string UNLOCK_KEY_PREFIX = "FormUnlocked_";

    public static bool IsUnlocked(int formID)
    {
        if (formID == 0) return true;
        return PlayerPrefs.GetInt(UNLOCK_KEY_PREFIX + formID, 0) == 1;
    }

    public static void Unlock(int formID)
    {
        PlayerPrefs.SetInt(UNLOCK_KEY_PREFIX + formID, 1);
        PlayerPrefs.Save();
        Debug.Log($"[FormUnlockManager] Form {formID} unlocked and saved");
    }

    public static void Lock(int formID)
    {
        if (formID == 0) return;
        PlayerPrefs.SetInt(UNLOCK_KEY_PREFIX + formID, 0);
        PlayerPrefs.Save();
    }

    public static void UnlockAll()
    {
        for (int i = 1; i <= 3; i++)
        {
            Unlock(i);
        }
    }

    public static void ResetAll()
    {
        for (int i = 1; i <= 3; i++)
        {
            Lock(i);
        }
    }
}
