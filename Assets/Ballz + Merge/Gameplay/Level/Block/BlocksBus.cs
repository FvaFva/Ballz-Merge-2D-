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
        [SerializeField] private BlockMagneticObserver _blockMagneticObserver;
        [SerializeField] private BlockAdditionalEffectHandler _additionalEffectHandler;

        [Inject] private GridSettings _gridSettings;
        [Inject] private BallWaveVolume _ballLevelVolume;
        [Inject] private BlocksInGame _activeBlocks;
        [Inject] private DiContainer _diContainer;

        private BallVolumeHitInspector _hitInspector;

        public event Action WaveSpawned;

        private void Awake()
        {
            _blockMagneticObserver = _diContainer.Instantiate<BlockMagneticObserver>();
        }

        private void OnEnable()
        {
            _blockMagneticObserver?.UpdateSubscribe(true);
            _additionalEffectHandler.BlockDestroyRequired += DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired += MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired += ChangeNumber;
            _activeBlocks.BlockHit += OnBlockHit;
            _activeBlocks.BlocksMerged += OnMergeBlocks;
        }

        private void OnDisable()
        {
            _blockMagneticObserver.UpdateSubscribe(false);
            _additionalEffectHandler.BlockDestroyRequired -= DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired -= MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired -= ChangeNumber;
            _activeBlocks.BlockHit -= OnBlockHit;
            _activeBlocks.BlocksMerged -= OnMergeBlocks;
        }

        public void Init()
        {
            _additionalEffectHandler.ConnectActiveBlocks(_activeBlocks);
            _hitInspector = new BallVolumeHitInspector(_activeBlocks, _ballLevelVolume, TryMoveBlock);
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

        public void StartSpawnWave(Action callBAck)
        {
            StartCoroutine(WaveGeneration(callBAck));
        }

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
            TryMoveBlock(block, direction);

            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Move, block, direction));
        }

        private void DestroyBlock(Block block)
        {
            block.Destroy();
            _effectsPool.SpawnEffect(BlockAdditionalEffectEvents.Destroy, block.WorldPosition);
            _destroyImpact.ShowImpact();
        }

        private bool TryMoveBlock(Block block, Vector2Int direction, float depth = 1)
        {
            if (depth == 0)
                return false;

            Vector2Int nextPosition = block.GridPosition + direction;

            if (nextPosition.x < 0 || nextPosition.y >= _gridSettings.GridSize.y || nextPosition.x >= _gridSettings.GridSize.x || direction == Vector2Int.down)
            {
                block.PlayBounceAnimation(direction);
                return false;
            }
            else if (_activeBlocks.GetAtPosition(block.GridPosition + direction) == null)
            {
                block.Move(direction);
                return true;
            }
            else
            {
                Block nextBlock = _activeBlocks.GetAtPosition(block.GridPosition + direction);

                if (TryMoveBlock(nextBlock, direction, --depth))
                {
                    if (TryMoveBlock(block, direction))
                    {
                        return true;
                    }
                }

                block.PlayBounceAnimation(direction);
                return false;
            }
        }

        private void MoveBlock(Block block, Vector2Int direction) => TryMoveBlock(block, direction);

        private void ChangeNumber(Block block, int count) => CheckChangeNumber(block, count, false);

        private bool CheckChangeNumber(Block block, int count, bool isHandleEvent = true)
        {
            block.ChangeNumber(count);

            if (block.Number == 0)
            {
                _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, block));
                DestroyBlock(block);
                return true;
            }

            if (isHandleEvent)
                _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.NumberChanged, block, count));

            return false;
        }

        private void OnMergeBlocks(Block firstBlock, Block secondBlock)
        {
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, firstBlock));
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, secondBlock));
            _mergeImpact.ShowImpact();
        }
    }
}
