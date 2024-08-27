using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksBus : CyclicBehaviour, ILevelFinisher, IInitializable
    {
        [SerializeField] private BlocksSpawner _spawner;
        [SerializeField] private ExplosionPool _explosionPool;
        [SerializeField] private BlocksMergeImpact _mergeImpact;
        [SerializeField] private BlockMagneticObserver _blockMagneticObserver;
        [SerializeField] private BlockAdditionalEffectHandler _additionalEffectHandler;

        [Inject] private Ball _ball;
        [Inject] private PhysicGrid _physicsGrid;
        [Inject] private GridSettings _gridSettings;
        [Inject] private BallWaveVolume _ballLevelVolume;

        private BlocksInGame _activeBlocks = new BlocksInGame();
        private BlocksMover _mover = new BlocksMover();
        private BallCollisionHandler _collisionHandler;

        public event Action BlockFinished;
        public event Action WaveLoaded;

        private void OnEnable()
        {
            if (_collisionHandler != null)
                _collisionHandler.HitBlock += OnBlockHit;

            _ball.EnterAim += OnStartLevel;
            _mover.BlockMoved += OnBlockComeToNewPosition;
            _mover.ChangedCellActivity += OnChangedCellActivity;
            _activeBlocks.ChangedCellActivity += OnChangedCellActivity;
            _blockMagneticObserver?.UpdateSubscribe(true);
            _additionalEffectHandler.BlockDestroyRequired += DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired += MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired += ChangeNumber;
        }

        private void OnDisable()
        {
            if (_collisionHandler != null)
                _collisionHandler.HitBlock -= OnBlockHit;

            _ball.EnterAim -= OnStartLevel;
            _mover.BlockMoved -= OnBlockComeToNewPosition;
            _mover.ChangedCellActivity -= OnChangedCellActivity;
            _activeBlocks.ChangedCellActivity -= OnChangedCellActivity;
            _blockMagneticObserver?.UpdateSubscribe(false);
            _additionalEffectHandler.BlockDestroyRequired -= DestroyBlock;
            _additionalEffectHandler.BlockMoveRequired -= MoveBlock;
            _additionalEffectHandler.BlockNumberChangedRequired -= ChangeNumber;
        }

        public void Init()
        {
            _collisionHandler = _ball.GetBallComponent<BallCollisionHandler>();
            _blockMagneticObserver = new BlockMagneticObserver(_collisionHandler);
            _collisionHandler.HitBlock += OnBlockHit;
            _additionalEffectHandler.ConnectActiveBlocks(_activeBlocks);
        }

        public void FinishLevel()
        {
            _mover.Clear();
            _activeBlocks.Clear();
        }

        private void OnBlockHit(GridCell cell, Vector2 hitPosition)
        {
            Block block = _activeBlocks.GetAtPosition(cell.GridPosition);

            if (block == null)
            {
                _physicsGrid.ChangeCellActivity(cell, false);
                Debug.Log($"hit invisible block - {cell.GridPosition}");
                return;
            }

            Vector2Int direction = hitPosition.CalculateDirection(block.WorldPosition);

            if (_ballLevelVolume.CheckVolume(BallVolumesTypes.Crush))
            {
                DestroyBlock(block);
                _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Destroy, block));
                return;
            }

            if (_blockMagneticObserver.CheckBlock(block, out Block secondBlock) && _ballLevelVolume.CheckVolume(BallVolumesTypes.Magnet))
            {
                MergeBlocks(block, secondBlock);
                return;
            }

            if (_ballLevelVolume.CheckVolume(BallVolumesTypes.NumberReductor))
            {
                if (CheckChangeNumber(block, -1))
                    return;
            }

            _additionalEffectHandler.HandleEvent(new(BlockAdditionalEffectEvents.Move, block, direction));
            TryMoveBlock(block, direction);
        }

        private void DestroyBlock(Block block)
        {
            _activeBlocks.Remove(block);
            block.Destroy();
            _explosionPool.SpawnEffect(block.WorldPosition);
        }

        private bool TryMoveBlock(Block block, Vector2Int direction)
        {
            if (direction == Vector2Int.down)
                return false;

            Vector2Int nextPosition = block.GridPosition + direction;

            if (nextPosition.x < 0 || nextPosition.y >= _gridSettings.GridSize.y || nextPosition.x >= _gridSettings.GridSize.x)
            {
                block.Shake(direction);
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
                block.Shake(direction);
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

        private void OnStartLevel()
        {
            _mover.MoveAllDawn(_activeBlocks.Items);

            if (_activeBlocks.TryDeactivateUnderLine(_gridSettings.LastRowIndex))
                BlockFinished?.Invoke();
            else
                StartCoroutine(DelayedWaveSpawn());
        }

        private IEnumerator DelayedWaveSpawn()
        {
            yield return new WaitForSeconds(_gridSettings.MoveTime);
            IEnumerable<Block> spawnBlocks = _spawner.SpawnWave();
            _activeBlocks.AddBlocks(spawnBlocks);
            WaveLoaded?.Invoke();
            _additionalEffectHandler.HandleWave(spawnBlocks);
        }

        private void OnBlockComeToNewPosition(Block block)
        {
            foreach (Vector2Int direction in new Vector2Int[4] { Vector2Int.right, Vector2Int.left, Vector2Int.down, Vector2Int.up })
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
