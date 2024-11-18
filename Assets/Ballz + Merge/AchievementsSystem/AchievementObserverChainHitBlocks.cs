using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

public class AchievementObserverChainHitBlocks : AchievementObserverBase
{
    [Inject] private Ball _ball;

    private BallCollisionHandler _ballCollisionHandler;

    public AchievementObserverChainHitBlocks(AchievementSettings settings) : base(settings)
    {

    }

    public override void Construct()
    {
        _ballCollisionHandler = _ball.GetBallComponent<BallCollisionHandler>();
        _ballCollisionHandler.GameZoneLeft += OnGameZoneLeft;
        _ballCollisionHandler.HitBlock += OnBlockHit;
    }

    protected override void Destruct()
    {
        _ballCollisionHandler.GameZoneLeft -= OnGameZoneLeft;
    }

    protected override void OnAchievementTargetReached(int target, int count, int maxTarget)
    {
        Debug.Log($"Вы уничтожили {count} блоков за раз и достигли {target} этапа из {maxTarget}");
    }

    private void OnBlockHit(GridCell cell, Vector2 contactPoint)
    {
        Property.Apply(Count);
    }

    private void OnGameZoneLeft()
    {
        Property.Reset();
    }
}
