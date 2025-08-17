using System;

public struct UserQuestion
{
    public string Description;
    public Action<bool> CallBack;

    public UserQuestion(Action<bool> callBack, string description)
    {
        Description = description;
        CallBack = callBack;
    }

    public bool IsEmpty()
    {
        return CallBack == null;
    }
}