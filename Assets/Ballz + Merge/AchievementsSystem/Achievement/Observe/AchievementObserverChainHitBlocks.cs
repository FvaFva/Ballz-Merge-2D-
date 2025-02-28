using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

namespace BallzMerge.Achievement
{
    public class AchievementObserverChainHitBlocks : AchievementObserverBase
    {
        [Inject] private Ball _ball;

        private BallCollisionHandler _ballCollisionHandler;
        private int _hitsInShot;

        public AchievementObserverChainHitBlocks(AchievementSettings settings, AchievementPointsStep pointsStep) : base(settings, pointsStep)
        {
        }

        public override void Construct()
        {
            _ballCollisionHandler = _ball.GetBallComponent<BallCollisionHandler>();
            _ballCollisionHandler.GameZoneLeft += OnGameZoneLeft;
           // _ballCollisionHandler.HitBlock += OnBlockHit;
        }

        protected override void Destruct()
        {
            _ballCollisionHandler.GameZoneLeft -= OnGameZoneLeft;
            //_ballCollisionHandler.HitBlock -= OnBlockHit;
        }

        private void OnBlockHit(GridCell cell, Vector2 contactPoint)
        {
            _hitsInShot += Count;

            if (_hitsInShot > Property.Values.Points)
                Property.Apply(Count);
        }

        private void OnGameZoneLeft()
        {
            _hitsInShot = 0;
        }
    }
}