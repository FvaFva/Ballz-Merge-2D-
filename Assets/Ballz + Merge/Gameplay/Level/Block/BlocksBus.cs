using System;
using UnityEngine;
using Zenject;

public class BlocksBus : CyclicBehaviour, ILevelFinisher
{
    [SerializeField] private BlocksSpawner _spawner;
    [SerializeField] private BlocksMergeImpact _mergeImpact;

    [Inject] private Ball _ball;
    [Inject] private PhysicGrid _physicsGrid;
    [Inject] private GridSettings _gridSettings;
    [Inject] private BallWaveVolume _ballLevelVolume;

    private BlocksInGame _activeBlocks = new BlocksInGame();
    private BlocksMover _mover = new BlocksMover();
    private BallCollisionHandler _collisionHandler;

    public event Action BlockFinished;
    public event Action WaveLoaded;

    private void Awake()
    {
        _collisionHandler = _ball.GetBallComponent<BallCollisionHandler>();
    }

    private void OnEnable()
    {
        _collisionHandler.HitBlock += OnBlockHit;
        _ball.EnterAim += OnStartLevel;
        _mover.BlockMoved += OnBlockComeToNewPosition;
        _mover.ChangedCellActivity += OnChangedCellActivity;
        _activeBlocks.ChangedCellActivity += OnChangedCellActivity;
    }

    private void OnDisable()
    {
        _collisionHandler.HitBlock -= OnBlockHit;
        _ball.EnterAim -= OnStartLevel;
        _mover.BlockMoved -= OnBlockComeToNewPosition;
        _mover.ChangedCellActivity -= OnChangedCellActivity;
        _activeBlocks.ChangedCellActivity -= OnChangedCellActivity;
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
            return;
        }

        if (_ballLevelVolume.CheckVolume(BallVolumesTypes.Crush))
        {
            _activeBlocks.Remove(block);
            block.Merge(block.WorldPosition);
            return;
        }

        if (_ballLevelVolume.CheckValue(BallVolumesTypes.NumberReductor))
        {
            block.ReduceNumber();
        }

        Vector2Int direction = hitPosition.CalculateDirection(block.WorldPosition);

        if (direction == Vector2Int.down)
            return;

        Vector2Int nextPosition = block.GridPosition + direction;

        if (nextPosition.x < 0 || nextPosition.y >= _gridSettings.GridSize.y || nextPosition.x >= _gridSettings.GridSize.x)
            block.Shake(direction);
        else
            HandleMove(block, direction);
    }

    private void HandleMove(Block block, Vector2Int direction)
    {
        if (_activeBlocks.GetAtPosition(block.GridPosition + direction) == null)
        {
            _mover.Move(block, direction);
        }
        else if (TryMergeCell(block, direction) == false)
        {
            block.Shake(direction);
        }
    }

    private bool TryMergeCell(Block block, Vector2Int direction)
    {
        Block blockInNextCell = _activeBlocks.GetAtPosition(block.GridPosition + direction);

        if (blockInNextCell != null && blockInNextCell.Number == block.Number)
        {
            _activeBlocks.Remove(blockInNextCell);
            _activeBlocks.Remove(block);
            _mover.Merge(blockInNextCell, block);
            _mergeImpact.ShowImpact();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnStartLevel()
    {
        _mover.MoveAllDawn(_activeBlocks.Items);

        if (_activeBlocks.TryDeactivateUnderLine(_gridSettings.LastRowIndex))
        {
            BlockFinished?.Invoke();
        }
        else
        {
            _activeBlocks.AddBlocks(_spawner.SpawnWave());
            WaveLoaded?.Invoke();
        }
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
