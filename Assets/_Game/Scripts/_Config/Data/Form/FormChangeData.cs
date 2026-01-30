public struct FormChangeData
{
    public int FromFormID;
    public int ToFormID;
    public string FromFormName;
    public string ToFormName;

    public FormChangeData(int fromID, int toID, string fromName, string toName)
    {
        FromFormID = fromID;
        ToFormID = toID;
        FromFormName = fromName;
        ToFormName = toName;
    }
}
