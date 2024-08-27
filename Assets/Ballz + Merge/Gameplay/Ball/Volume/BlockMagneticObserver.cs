using BallzMerge.Gameplay.BlockSpace;

namespace BallzMerge.Gameplay.BallSpace
{
    public class BlockMagneticObserver
    {
        private Block _lastBlock;
        private BallCollisionHandler _collisionHandler;

        public BlockMagneticObserver(BallCollisionHandler collisionHandler)
        {
            _collisionHandler = collisionHandler;
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
