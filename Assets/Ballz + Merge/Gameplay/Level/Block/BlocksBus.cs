using System.Collections.Generic;
using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksBus : CyclicBehavior, IInitializable
    {
        [SerializeField] private BlocksSpawner _spawner;
        [SerializeField] private AdditionalEffectsPool _effectsPool;
        [SerializeField] private BlocksMergeImpact _mergeImpact;
        [SerializeField] private BlocksDestroyImpact _destroyImpact;
        [SerializeField] private BlockAdditionalEffectHandler _additionalEffectHandler;

        [Inject] private GridSettings _gridSettings;
        [Inject] private BlocksInGame _activeBlocks;
        [Inject] private DiContainer _diContainer;

        private BallVolumeHitInspector _hitInspector;
        private BlocksMover _mover;

        public event Action WaveSpawned;

        private void Awake()
        {
            _mover = _diContainer.Instantiate<BlocksMover>();
            _hitInspector = _diContainer.Instantiate<BallVolumeHitInspector>(new object[] { _mover });
        }

        private void OnEnable()
        {
            _additionalEffectHandler.BlockDestroyRequired += OnDestroyBlock;
            _additionalEffectHandler.BlockMoveRequired += MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired += OnChangeNumber;
            _activeBlocks.BlockHit += OnBlockHit;
            _activeBlocks.BlocksMerged += OnMergeBlocks;
            _activeBlocks.BlockDestroyed += OnDestroyBlock;
        }

        private void OnDisable()
        {
            _additionalEffectHandler.BlockDestroyRequired -= OnDestroyBlock;
            _additionalEffectHandler.BlockMoveRequired -= MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired -= OnChangeNumber;
            _activeBlocks.BlockHit -= OnBlockHit;
            _activeBlocks.BlocksMerged -= OnMergeBlocks;
            _activeBlocks.BlockDestroyed -= OnDestroyBlock;
        }

        public void Init()
        {
            _additionalEffectHandler.ConnectActiveBlocks(_activeBlocks);
        }

        public bool TryFinish()
        {
            if(_activeBlocks.TryDeactivateUnderLine(_gridSettings.LastRowIndex))
            {
                _activeBlocks.Clear();
                return true;
            }
            else
            {
                MoveAllBlocks(Vector2Int.down);
                return false;
            }
        }

        public void MoveAllBlocks(Vector2Int direction)
        {
            foreach(var block in _activeBlocks.Blocks)
                block.Move(direction);
        }

        public void StartSpawnWave(Action callBAck) => StartCoroutine(WaveGeneration(callBAck));

        private IEnumerator WaveGeneration(Action callBack)
        {
            var delay = new WaitForSeconds(0.05f);
            var spawnBlocks = new List<Block>();

            foreach (var block in _spawner.SpawnWave())
            {
                spawnBlocks.Add(block);
                yield return delay;
            }

            yield return delay;
            _additionalEffectHandler.HandleWave(spawnBlocks);
            yield return delay;
            WaveSpawned?.Invoke();
            callBack();
        }

        private void OnBlockHit(Block block, Vector2Int direction)
        {
            var data = new BallVolumeHitData();
            data.Direction = direction;
            data.Block = block;
            _hitInspector.Explore(data);
            _mover.Try(block, direction);

            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Move, block, direction));
        }

        private void OnDestroyBlock(Block block)
        {
            _effectsPool.SpawnEffect(BlockAdditionalEffectEvents.Destroy, block.WorldPosition);
            _destroyImpact.ShowImpact();
        }

        private void MoveBlock(Block block, Vector2Int direction) => _mover.Try(block, direction);

        private void OnChangeNumber(Block block, int count) => _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.NumberChanged, block, count));

        private void OnMergeBlocks(Block firstBlock, Block secondBlock)
        {
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, firstBlock));
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, secondBlock));
            _mergeImpact.ShowImpact();
        }
    }
}
