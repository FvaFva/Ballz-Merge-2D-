using BallzMerge.Gameplay.BlockSpace;
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
}
