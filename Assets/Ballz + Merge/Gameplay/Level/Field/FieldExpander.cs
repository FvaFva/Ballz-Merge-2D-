using BallzMerge.Data;
using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IInitializable, IWaveUpdater, ILevelSaver, ILevelLoader, ILevelFinisher
{
    private const string FieldSizeX = "FieldSizeX";
    private const string FieldSizeY = "FieldSizeY";
    private const float FrameSize = 3.8f;

    [SerializeField] private PlayZoneBoards _boards;
    [SerializeField] private FieldExpanderSettings _fieldExpanderSettings;
    [SerializeField] private ParticleSystem _fieldEffect;
    [SerializeField] private CamerasOperator _cameras;

    [Inject] private PhysicGrid _physicGrid;
    [Inject] private DataBaseSource _data;
    [Inject] private GridSettings _gridSettings;
    [Inject] private BlocksBinder _blocksBinder;
    [Inject] private BallWaveVolume _ballWaveVolume;

    private int _remainingCountSpawn;
    private int _currentColumns;
    private int _currentRows;
    private int _countUntilSpawn;
    private float _halfSize;
    private float _sizeWithSpace;

    private ParticleSystem.ShapeModule _fieldShape;
    private Vector2 _fieldPosition;
    private Vector2 _fieldScale;
    private PositionScaleProperty _propertyColumn;
    private PositionScaleProperty _propertyRow;

    public void Init()
    {
        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _halfSize = _gridSettings.CellSize / 2;
        _sizeWithSpace = _gridSettings.CellSize + _gridSettings.CellSpacing;
        _propertyRow = new PositionScaleProperty(0, _halfSize, 0, _gridSettings.CellSize);
        _propertyColumn = new PositionScaleProperty(_halfSize, 0, _gridSettings.CellSize, 0);
    }

    public void UpdateWave()
    {
        if (++_countUntilSpawn == _fieldExpanderSettings.CountUntilSpawn)
        {
            if (_fieldExpanderSettings.IsLoop)
            {
                SpawnColumn();
            }
            else
            {
                if (_remainingCountSpawn > 0)
                {
                    _remainingCountSpawn--;
                    SpawnColumn();
                }
            }
        }
    }

    public void StartLevel()
    {
        _gridSettings.ReloadSize();
        _physicGrid.InitGrid();
        _ballWaveVolume.Bag.Added += OnAbilityAdd;
        _fieldEffect.transform.position = _fieldPosition;
        _fieldShape.scale = _fieldScale;
        _countUntilSpawn = 0;
        _remainingCountSpawn = _fieldExpanderSettings.CountOfSpawn;
        _currentColumns = _gridSettings.Size.x;
        _currentRows = _gridSettings.Size.y;
        _cameras.SetGameplayBoardSize(BoardSize());
    }

    public void GetSavingData()
    {
        _data.Saves.Save(new KeyValuePair<string, float>(FieldSizeX, _currentColumns));
        _data.Saves.Save(new KeyValuePair<string, float>(FieldSizeY, _currentRows));
    }

    public void Load()
    {
        StartLevel();

        _currentColumns = Mathf.RoundToInt(_data.Saves.Get(FieldSizeX));

        for (int i = _gridSettings.Size.x; i < _currentColumns; i++)
            AddColumn();
    }

    public void FinishLevel()
    {
        _gridSettings.ReloadSize();
        _ballWaveVolume.Bag.Added -= OnAbilityAdd;
        SetDefault();
    }

    private void OnAbilityAdd(BallVolumesBagCell volume)
    {
        if (volume.IsEqual(BallVolumesTypes.FieldExpander))
        {
            _currentRows++;
            AddRow();
        }
    }

    private void SpawnColumn()
    {
        _countUntilSpawn = 0;
        _currentColumns++;
        AddColumn();
    }

    private void AddColumn()
    {
        _physicGrid.SpawnColumn();
        SetComponentsPosition(_propertyColumn);
        _boards.ChangePositionScale(true, _propertyColumn);
    }

    private void AddRow()
    {
        _physicGrid.SpawnRow();
        _blocksBinder.StartMoveAllBlocks(Vector2Int.up, () => { return; });
        SetComponentsPosition(_propertyRow);
        _boards.ChangePositionScale(false, _propertyRow);
    }

    private void SetComponentsPosition(PositionScaleProperty property)
    {
        _fieldEffect.transform.position += property.Position;
        _fieldShape.scale += property.Scale;
        _cameras.AddValue(_cameras.Gameplay, position: property.Position);
        _cameras.SetGameplayBoardSize(BoardSize());
    }

    private Vector2 BoardSize()
    {
        return new Vector2(_sizeWithSpace * _currentColumns + FrameSize, _sizeWithSpace * _currentRows + FrameSize);
    }

    private void SetDefault()
    {
        _fieldEffect.transform.position = _fieldPosition;
        _fieldShape.scale = _fieldScale;
        _cameras.SetDefault();
        _countUntilSpawn = 0;
        _remainingCountSpawn = _fieldExpanderSettings.CountOfSpawn;
        _currentColumns = _gridSettings.Size.x;
        _currentRows = _gridSettings.Size.y;
    }
}