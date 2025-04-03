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
        private bool _isActive;

        public override void HandleWave()
        {
            if (_connectBlock == null || Current == null)
                Deactivate();
        }

        protected override bool TryActivate()
        {
            _connectBlock = ActiveBlocks.GetRandomBlock(Current);
            _blocksSubscriptionStates.Add(Current, false);

            if (_connectBlock == null)
                return false;

            _isActive = true;
            ChangeViewActivity(true);
            Current.ConnectEffect();
            _connectBlock.ConnectEffect();
            _connectBlock.Deactivated += HandleDeactivateBlock;
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

        protected override void HandleDeactivate() => HandleDeactivateBlock(Current);

        private void HandleDeactivateBlock(Block block)
        {
            if (_isActive)
            {
                _isActive = false;
                ChangeViewActivity(false);
                UpdateSubscription(_connectBlock, false);
                UpdateSubscription(Current, false);
                _connectBlock.Deactivated -= HandleDeactivateBlock;
                _blocksSubscriptionStates.Clear();
                Another(block).Destroy();
            }
        }

        private void UpdateSubscription(Block block, bool isActive)
        {
            if(block == null || _blocksSubscriptionStates.ContainsKey(block) == false) 
                return;

            if (_blocksSubscriptionStates[block] == isActive)
                return;
            else
                _blocksSubscriptionStates[block] = isActive;

            if (isActive)
            {
                block.Hit += OnHit;
                block.NumberChanged += OnNumberChanged;
            }
            else
            {
                block.Hit -= OnHit;
                block.NumberChanged -= OnNumberChanged;
            }
        }

        private void OnNumberChanged(Block block, int Count)
        {
            var another = Another(block);
            block.NumberChanged -= OnNumberChanged;
            another.ChangeNumber(Count);
            block.NumberChanged += OnNumberChanged;
        }

        private void OnHit(Block block, Vector2Int step) => Another(block).Move(step);

        private Block Another(Block block) => block == Current ? _connectBlock : Current;

        private void ChangeViewActivity(bool isActive)
        {
            _renderer.enabled = isActive;

            if (isActive)
            {
                _particleLast.Play();
                _particleFirst.Play();
            }
            else
            {
                _particleLast.Stop();
                _particleFirst.Stop();
            }
        }
    }
}
