using BallzMerge.Gameplay.BlockSpace;
using Zenject;

namespace BallzMerge.Gameplay.BallSpace
{
    public class BlockMagneticObserver
    {
        private Block _lastBlock;
        private BallCollisionHandler _collisionHandler;

        [Inject]
        public BlockMagneticObserver(Ball ball)
        {
            _collisionHandler = ball.GetBallComponent<BallCollisionHandler>();
            _collisionHandler.NonBlockHit += Clear;
        }

        public void UpdateSubscribe(bool isSubscribed)
        {
            if (isSubscribed)
                _collisionHandler.NonBlockHit += Clear;
            else
                _collisionHandler.NonBlockHit -= Clear;
        }

        public bool CheckBlock(Block checkedBlock, out Block lastBlock)
        {
            if (_lastBlock != null && _lastBlock != checkedBlock && _lastBlock.Number == checkedBlock.Number)
            {
                lastBlock = _lastBlock;
                Clear();
                return true;
            }

            _lastBlock = checkedBlock;
            lastBlock = null;
            return false;
        }

        public void Clear()
        {
            _lastBlock = null;
        }
    }
}
