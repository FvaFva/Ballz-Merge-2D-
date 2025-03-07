using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockAdditionalEffectBandler : BlockAdditionalEffectBase
    {
        private const int FirstIndex = 0;
        private const int SecondIndex = 1;

        [SerializeField] private LineRenderer _renderer;
        [SerializeField] private ParticleSystem _particleFirst;
        [SerializeField] private ParticleSystem _particleLast;

        private Block _connectBlock;
        private Transform _particleFirstTransform;
        private Transform _particleLastTransform;
        private Dictionary<Block, bool> _blocksSubscriptionStates = new Dictionary<Block, bool>();

        public override void HandleWave()
        {
            if (_connectBlock == null || Current == null)
                Deactivate();
        }

        protected override bool TryActivate()
        {
            _connectBlock = ActiveBlocks.GetRandomBlock(Current);

            if (_connectBlock == null)
                return false;

            Current.ConnectEffect();
            _connectBlock.ConnectEffect();
            _blocksSubscriptionStates.Add(Current, false);
            _blocksSubscriptionStates.Add(_connectBlock, false);
            UpdateSubscription(Current, true);
            UpdateSubscription(_connectBlock, true);
            return true;
        }

        protected override void HandleUpdate()
        {
            _renderer.SetPosition(FirstIndex, Current.WorldPosition);
            _renderer.SetPosition(SecondIndex, _connectBlock.WorldPosition);
            _particleFirstTransform.position = Current.WorldPosition;
            _particleLastTransform.position = _connectBlock.WorldPosition;
        }

        protected override void Init()
        {
            _particleFirstTransform = _particleFirst.transform;
            _particleLastTransform = _particleLast.transform;
        }

        protected override void HandleDeactivate()
        {
            UpdateSubscription(_connectBlock, false);
            UpdateSubscription(Current, false);
            _blocksSubscriptionStates.Clear();
        }

        private void UpdateSubscription(Block block, bool isActive)
        {
            if(block == null) 
                return;

            if (_blocksSubscriptionStates[block] == isActive)
                return;
            else
                _blocksSubscriptionStates[block] = isActive;

            if (isActive)
            {
                block.Moved += OnMoved;
                block.Destroyed += OnDestroyed;
                block.NumberChanged += OnNumberChanged;
            }
            else
            {
                block.Moved -= OnMoved;
                block.Destroyed -= OnDestroyed;
                block.NumberChanged -= OnNumberChanged;
            }
        }

        private void OnNumberChanged(Block block, int Count)
        {
            var another = block == Current ? _connectBlock : Current;
            UpdateSubscription(another, false);
            another.ChangeNumber(Count);

            if(another.IsAlive)
                UpdateSubscription(another, true);
            else
                block.Destroy();
        }

        private void OnDestroyed(Block block)
        {
            var another = block == Current ? _connectBlock : Current;
            UpdateSubscription(another, false);
            another.Destroy();
        }

        private void OnMoved(Block block, Vector2Int step)
        {
            if (step == Vector2Int.down)
                return;

            var another = block == Current ? _connectBlock : Current;
            another.Moved -= OnMoved;
            Mover.Try(another, step);
            another.Moved += OnMoved;
        }
    }
}
