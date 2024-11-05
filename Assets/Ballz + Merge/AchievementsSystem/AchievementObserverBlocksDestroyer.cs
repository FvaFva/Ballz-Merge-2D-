using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;
using Zenject;

public class AchievementObserverBlocksDestroyer : AchievementObserverBase
{
    [Inject] private BlocksInGame _blocks;

    private int _count;
    private AchievementProperty _property = new AchievementProperty();

    public AchievementObserverBlocksDestroyer(AchievementSettings settings, int count) : base(settings)
    {
        _blocks.BlocksDestroyed += OnBlockDestroyed;
        _property.Settings = settings;
        _property.Reached += OnAchievementTargetReached;
        _count = count;
    }

    protected override void Destruct()
    {
        _blocks.BlocksDestroyed -= OnBlockDestroyed;
    }

    private void OnBlockDestroyed()
    {
        _property.Apply(_count);
    }

    private void OnAchievementTargetReached(int target, int count, int maxTarget)
    {
        Debug.Log($"Вы уничтожили {count} блоков и достигли {target} этапа из {maxTarget}");
    }
}
