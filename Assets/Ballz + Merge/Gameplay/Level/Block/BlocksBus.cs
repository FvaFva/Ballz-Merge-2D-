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
        private const int CountTriesToCheckMovableBlocks = 2000;

        private readonly Vector2Int[] AllSides = new Vector2Int[4] { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up };

        [SerializeField] private BlocksSpawner _spawner;
        [SerializeField] private EffectsPool _effectsPool;
        [SerializeField] private BlocksMergeImpact _mergeImpact;
        [SerializeField] private BlocksDestroyImpact _destroyImpact;
        [SerializeField] private BlockMagneticObserver _blockMagneticObserver;
        [SerializeField] private BlockAdditionalEffectHandler _additionalEffectHandler;

        [Inject] private Ball _ball;
        [Inject] private PhysicGrid _physicsGrid;
        [Inject] private GridSettings _gridSettings;
        [Inject] private BallWaveVolume _ballLevelVolume;

        private BlocksInGame _activeBlocks = new BlocksInGame();
        private BallCollisionHandler _collisionHandler;
        private BlocksMover _mover = new BlocksMover();

        private void Awake()
        {
            _collisionHandler = _ball.GetBallComponent<BallCollisionHandler>();
            _blockMagneticObserver = new BlockMagneticObserver(_collisionHandler);
        }

        private void OnEnable()
        {
            _blockMagneticObserver?.UpdateSubscribe(true);
            _collisionHandler.HitBlock += OnBlockHit;
            _mover.BlockMoved += OnBlockComeToNewPosition;
            _mover.ChangedCellActivity += OnChangedCellActivity;
            _activeBlocks.ChangedCellActivity += OnChangedCellActivity;
            _additionalEffectHandler.BlockDestroyRequired += DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired += MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired += ChangeNumber;
        }

        private void OnDisable()
        {
            _blockMagneticObserver.UpdateSubscribe(false);
            _collisionHandler.HitBlock -= OnBlockHit;
            _mover.BlockMoved -= OnBlockComeToNewPosition;
            _mover.ChangedCellActivity -= OnChangedCellActivity;
            _activeBlocks.ChangedCellActivity -= OnChangedCellActivity;
            _additionalEffectHandler.BlockDestroyRequired -= DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired -= MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired -= ChangeNumber;
        }

        public void Init()
        {
           _additionalEffectHandler.ConnectActiveBlocks(_activeBlocks);
        }

        public bool TryFinish()
        {
            MoveAllBlocks(Vector2Int.down);

            if(_activeBlocks.TryDeactivateUnderLine(_gridSettings.LastRowIndex))
            {
                _mover.Clear();
                _activeBlocks.Clear();
                return true;
            }

            return false;
        }

        public void MoveAllBlocks(Vector2Int direction)
        {
            _mover.MoveAllDirection(_activeBlocks.Items, direction);
        }

        public void StartSpawnWave(Action callBAck)
        {
            StartCoroutine(WaveGeneration(callBAck));
        }

        private IEnumerator WaveGeneration(Action callBack)
        {
            int tries = CountTriesToCheckMovableBlocks;
            var delay = new WaitForSeconds(0.05f);

            while (_mover.CheckCorrectRestPosition() == false && --tries > 0)
                yield return delay;

            _mover.Clear();
            var spawnBlocks = new List<Block>();

            foreach (var block in _spawner.SpawnWave())
            {
                spawnBlocks.Add(block);
                yield return delay;
            }

            _activeBlocks.AddBlocks(spawnBlocks);
            yield return delay;
            _additionalEffectHandler.HandleWave(spawnBlocks);

            yield return new WaitForSeconds(_gridSettings.MoveTime);
            callBack();
        }

        private void OnBlockHit(GridCell cell, Vector2 hitPosition)
        {
            Block block = _activeBlocks.GetAtPosition(cell.GridPosition);

            if (block == null)
            {
                _physicsGrid.ChangeCellActivity(cell, false);
                Debug.Log($"hit-invalid block - {cell.GridPosition}");
                return;
            }

            Vector2Int direction = hitPosition.CalculateDirection(block.WorldPosition);

            if (_ballLevelVolume.GetCageValue(BallVolumesTypes.Crush) != 0)
            {
                DestroyBlock(block);
                _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, block));
                return;
            }

            if (_blockMagneticObserver.CheckBlock(block, out Block secondBlock) && _ballLevelVolume.GetCageValue(BallVolumesTypes.Magnet) != 0)
            {
                MergeBlocks(block, secondBlock);
                return;
            }

            if (_ballLevelVolume.GetCageValue(BallVolumesTypes.NumberReductor) != 0)
            {
                if (CheckChangeNumber(block, -1))
                    return;
            }

            if (_ballLevelVolume.GetCageValue(BallVolumesTypes.MoveIncreaser) != 0)
                TryMoveBlock(block, direction, _ballLevelVolume.GetPassiveValue(BallVolumesTypes.MoveIncreaser));
            else
                TryMoveBlock(block, direction);

            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Move, block, direction));

        }

        private void DestroyBlock(Block block)
        {
            _activeBlocks.Remove(block);
            _mover.ProcessDeleteBlock(block);
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
                block.ShakeDirection(direction);
                return false;
            }
            else if (_activeBlocks.GetAtPosition(block.GridPosition + direction) == null)
            {
                _mover.Move(block, direction);
                return true;
            }
            else if (TryMergeCell(block, direction))
            {
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

                block.ShakeDirection(direction);
                return false;
            }
        }

        private void MoveBlock(Block block, Vector2Int direction)
        {
            TryMoveBlock(block, direction);
        }

        private void ChangeNumber(Block block, int count)
        {
            CheckChangeNumber(block, count, false);
        }

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

        private bool TryMergeCell(Block block, Vector2Int direction)
        {
            Block blockInNextCell = _activeBlocks.GetAtPosition(block.GridPosition + direction);

            if (blockInNextCell != null && blockInNextCell.Number == block.Number)
            {
                MergeBlocks(block, blockInNextCell);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void MergeBlocks(Block firstBlock, Block secondBlock)
        {
            _mover.Merge(secondBlock, firstBlock);
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, firstBlock));
            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, secondBlock));
            _activeBlocks.Remove(secondBlock);
            _activeBlocks.Remove(firstBlock);
            _mergeImpact.ShowImpact();
        }

        private void OnBlockComeToNewPosition(Block block)
        {
            foreach (Vector2Int direction in AllSides)
            {
                if (TryMergeCell(block, direction))
                    return;
            }
        }

        private void OnChangedCellActivity(Vector2Int cell, bool isActive)
        {
            _physicsGrid.ChangeCellActivity(cell, isActive);
        }
    }
}
