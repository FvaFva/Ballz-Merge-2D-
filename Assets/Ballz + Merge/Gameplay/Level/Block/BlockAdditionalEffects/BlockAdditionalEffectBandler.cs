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

        private Transform _particleFirstTransform;
        private Transform _particleLastTransform;
        private Dictionary<Block, bool> _blocksSubscriptionStates = new Dictionary<Block, bool>();
        private bool _isActive;

        public override void HandleWave()
        {
            if (ConnectBlock == null || Current == null)
                Deactivate();
        }

        protected override bool TryActivate()
        {
            SetConnectBlock();
            _blocksSubscriptionStates.Add(Current, false);

            if (ConnectBlock == null)
                return false;

            _isActive = true;
            ChangeViewActivity(true);
            Current.ConnectEffect();
            ConnectBlock.ConnectEffect();
            ConnectBlock.Deactivated += HandleDeactivateBlock;
            _blocksSubscriptionStates.Add(ConnectBlock, false);
            UpdateSubscription(Current, true);
            UpdateSubscription(ConnectBlock, true);
            return true;
        }

        protected override void HandleUpdate()
        {
            _renderer.SetPosition(FirstIndex, Current.WorldPosition);
            _renderer.SetPosition(SecondIndex, ConnectBlock.WorldPosition);
            _particleFirstTransform.position = Current.WorldPosition;
            _particleLastTransform.position = ConnectBlock.WorldPosition;
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
                UpdateSubscription(ConnectBlock, false);
                UpdateSubscription(Current, false);
                ConnectBlock.Deactivated -= HandleDeactivateBlock;
                _blocksSubscriptionStates.Clear();
                GetOppositeCurrentOrConnection(block).Destroy();
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
            var another = GetOppositeCurrentOrConnection(block);
            block.NumberChanged -= OnNumberChanged;
            another.ChangeNumber(Count);
            block.NumberChanged += OnNumberChanged;
        }

        private void OnHit(Block block, Vector2Int step)
        {
            Block currentOrConnectionBlock = GetOppositeCurrentOrConnection(block);

            if (currentOrConnectionBlock.CanMove(step))
                currentOrConnectionBlock.Move(step);
        }

        private Block GetOppositeCurrentOrConnection(Block block) => block == Current ? ConnectBlock : Current;

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
