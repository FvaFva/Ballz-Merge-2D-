using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace BallzMerge.Achievement
{
    public struct AchievementData
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public bool IsWithIcon;
        public bool IsSecret;

        public AchievementData(IAchievementDescription description)
        {
            Name = description.title;
            Description = description.achievedDescription;
            IsSecret = description.hidden;
            IsWithIcon = description.image != null;

            if (IsWithIcon)
                Icon = Sprite.Create(description.image, new Rect(0, 0, description.image.width, description.image.height), new Vector2(0.5f, 0.5f));
            else
                Icon = null;
        }
    }
}