using UnityEngine;

public struct AchievementData
{
    public string Name;
    public string Description;
    public Sprite Image;
    public int MaxTargets;

    public AchievementData(string name, string description, Sprite image, int maxTargets)
    {
        Name = name;
        Description = description;
        Image = image;
        MaxTargets = maxTargets;
    }
}
