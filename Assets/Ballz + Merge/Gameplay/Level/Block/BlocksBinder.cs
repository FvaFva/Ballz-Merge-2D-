using System.Collections.Generic;
using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksBinder : CyclicBehavior, IInitializable, ISaveDependedObject, ICompleteLevelTrigger
    {
        private const float AnimationDelay = 0.1f;

        [SerializeField] private BlocksSpawner _spawner;
        [SerializeField] private AdditionalEffectsPool _effectsPool;
        [SerializeField] private BlocksMergeImpact _mergeImpact;
        [SerializeField] private BlocksDestroyImpact _destroyImpact;
        [SerializeField] private BlockAdditionalEffectHandler _additionalEffectHandler;

        [Inject] private GridSettings _gridSettings;
        [Inject] private BlocksInGame _activeBlocks;
        [Inject] private DiContainer _diContainer;
        [Inject] private BlocksMover _mover;

        private BallVolumeHitInspector _hitInspector;
        private WaitForSeconds _sleep;

        public bool IsReadyToComplete => _activeBlocks.NoBlocks;
        public event Action WaveSpawned;

        private void Awake()
        {
            _hitInspector = _diContainer.Instantiate<BallVolumeHitInspector>();
            _additionalEffectHandler.Init(_activeBlocks);
        }

        private void OnEnable()
        {
            _activeBlocks.BlockHit += OnBlockHit;
            _activeBlocks.BlocksMerged += OnMergeBlocks;
            _activeBlocks.BlockDestroyed += OnDestroyBlock;
        }

        private void OnDisable()
        {
            _activeBlocks.BlockHit -= OnBlockHit;
            _activeBlocks.BlocksMerged -= OnMergeBlocks;
            _activeBlocks.BlockDestroyed -= OnDestroyBlock;
        }

        public void Init()
        {
            _sleep = new WaitForSeconds(AnimationDelay);
            _hitInspector.Init();
        }

        public void Save(SaveDataContainer save)
        {
            foreach (Block block in _activeBlocks.Blocks)
                save.Blocks.Add(new SavedBlock(block.ID, block.Number, block.GridPosition.x, block.GridPosition.y));
        }

        public void Load(SaveDataContainer save)
        {
            foreach (SavedBlock savedBlock in save.Blocks)
                _spawner.SpawnBlock(savedBlock.Number, new Vector2Int(savedBlock.GridPositionX, savedBlock.GridPositionY), savedBlock.ID);

            _additionalEffectHandler.LoadEffects(_activeBlocks.Blocks);
            _spawner.ResetBlocksID();
        }

        public bool TryFinish()
        {
            if (_activeBlocks.TryDeactivateUnderLine(_gridSettings.LastRowIndex))
            {
                _activeBlocks.Clear();
                return true;
            }

            return false;
        }

        public void StartMoveAllBlocks(Vector2Int direction, Action callBack) => StartCoroutine(BlocksMoving(direction, callBack));

        public void StartSpawnWave(Action callBAck) => StartCoroutine(WaveGeneration(callBAck));

        private IEnumerator BlocksMoving(Vector2Int direction, Action callBack)
        {
            foreach (var _ in _mover.MoveAll(_activeBlocks.Blocks, direction, callBack))
                yield return _sleep;
        }

        private IEnumerator WaveGeneration(Action callBack)
        {
            var spawnBlocks = new List<Block>();

            foreach (var block in _spawner.SpawnWave())
            {
                spawnBlocks.Add(block);
                yield return _sleep;
            }

            yield return _sleep;
            _additionalEffectHandler.HandleWave(spawnBlocks);
            yield return _sleep;
            WaveSpawned?.Invoke();
            callBack();
        }

        private void OnBlockHit(Block block, Vector2Int direction)
        {
            var data = new BallVolumeHitData();
            data.Direction = direction;
            data.Block = block;
            _hitInspector.Explore(data);

            if (block.CanMove(direction))
                block.Move(direction, BlockMoveActionType.ChangePosition);
        }

        private void OnDestroyBlock(Block block)
        {
            _effectsPool.SpawnEffect(BlockAdditionalEffectEvents.Destroy, block.WorldPosition);
            _destroyImpact.ShowImpact();
        }

        private void OnMergeBlocks(Block firstBlock, Block secondBlock)
        {
            _mergeImpact.ShowImpact();
        }
    }
}
