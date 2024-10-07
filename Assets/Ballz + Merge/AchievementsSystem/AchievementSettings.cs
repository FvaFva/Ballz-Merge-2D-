using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement settings", menuName = "Bellz+Merge/Achievements/AchievementSettings", order = 51)]
public class AchievementSettings : ScriptableObject
{
    [SerializeField] private List<int> _targets;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private Sprite _image;

    public int CheckReachedTarget(int target)
    {
        for (int i = 0; i < _targets.Count; i++)
        {
            if (_targets[i] == target)
                return i + 1;
        }

        return 0;
    }
}
