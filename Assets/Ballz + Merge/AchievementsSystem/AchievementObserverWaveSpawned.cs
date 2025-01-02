using BallzMerge.Gameplay.BlockSpace;
using UnityEngine;
using Zenject;

public class AchievementObserverWaveSpawned : AchievementObserverBase
{
    [Inject] private BlocksBus _blocksBus;

    public AchievementObserverWaveSpawned(AchievementSettings settings) : base(settings)
    {

    }

    public override void Construct()
    {
        _blocksBus.WaveSpawned += OnWaveSpawned;
    }

    protected override void Destruct()
    {
        _blocksBus.WaveSpawned -= OnWaveSpawned;
    }

    protected override void OnAchievementTargetReached(int target, int count, int maxTarget)
    {
        Debug.Log($"Появилось {count} волн. Вы достигли {target} этапа из {maxTarget}");
    }

    private void OnWaveSpawned()
    {
        Property.Apply(Count);
    }
}
