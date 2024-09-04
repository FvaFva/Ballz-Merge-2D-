using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectBandler : BlockAdditionalEffectBase
    {
        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private ParticleSystem _particleFirst;
        [SerializeField] private ParticleSystem _particleLast;

        private Block _connectBlock;

        private void FixedUpdate()
        {
            if (Current != null && _connectBlock != null)
            {
                _renderer.SetPosition(0, Current.WorldPosition);
                _renderer.SetPosition(1, _connectBlock.WorldPosition);
                _particleFirst.transform.position = Current.WorldPosition;
                _particleLast.transform.position = _connectBlock.WorldPosition;
            }
        }

        public override void HandleWave()
        {
            if (_connectBlock == null || Current == null)
                Deactivate();
        }

        protected override bool TryInit(BlocksInGame blocks)
        {
            if (blocks == null)
                return false;

            _connectBlock = blocks.GetRandomBlock(Current);

            if (_connectBlock == null)
                return false;

            Current.ConnectEffect();
            _connectBlock.ConnectEffect();
            return true;
        }

        private Block GetAnotherBlock(Block block)
        {
            return block == Current ? _connectBlock : Current;
        }

        private void OnBlockDestroyed(Block block)
        {
            _connectBlock = null;
            InvokeActionBlockDestroyed(block);
            Deactivate();
        }

        public override void HandleEvent(BlockAdditionalEffectEventProperty property)
        {
            if (property.Current == Current || property.Current == _connectBlock)
            {
                Block block = GetAnotherBlock(property.Current);

                switch (property.EffectEvents)
                {
                    case BlockAdditionalEffectEvents.Move:
                        InvokeActionBlockMoved(block, property.Direction);
                        break;
                    case BlockAdditionalEffectEvents.NumberChanged:
                        InvokeActionNumberChanged(block, property.Count, false);
                        break;
                    case BlockAdditionalEffectEvents.Destroy:
                        OnBlockDestroyed(block);
                        break;
                }
            }
        }
    }
}
