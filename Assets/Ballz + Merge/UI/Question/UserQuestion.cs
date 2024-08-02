public struct UserQuestion
{
    public string Name;
    public string Description;
    public bool IsPositiveAnswer;

    public UserQuestion(string name, string description)
    {
        Name = name;
        Description = description;
        IsPositiveAnswer = false;
    }

    public bool IsEmpty()
    {
        return Name == null || Name.Equals(string.Empty);
    }
}