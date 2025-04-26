using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter, ILevelFinisher, ILevelSaver
{
    private const float FrameSize = 3.8f;
    private const string FieldEffectPositionX = "FieldEffectPositionX";
    private const string FieldEffectPositionY = "FieldEffectPositionY";
    private const string FieldEffectScaleX = "FieldEffectScaleX";
    private const string FieldEffectScaleY = "FieldEffectScaleY";
    private const string CameraOrthographicSize = "CameraOrthographicSize";
    private const string CameraPositionX = "CameraPositionX";
    private const string CameraPositionY = "CameraPositionY";

    [SerializeField] private PlayZoneBoards _boards;
    [SerializeField] private FieldExpanderSettings _fieldExpanderSettings;
    [SerializeField] private ParticleSystem _fieldEffect;
    [SerializeField] private CamerasOperator _cameras;

    [Inject] private PhysicGrid _physicGrid;
    [Inject] private GridSettings _gridSettings;
    [Inject] private BlocksBinder _blocksBinder;
    [Inject] private BallWaveVolume _ballWaveVolume;

    private int _count;
    private int _extraColumns;
    private int _extraRows;
    private int _currentWave;
    private float _halfSize;
    private float _sizeWithSpace;

    private ParticleSystem.ShapeModule _fieldShape;
    private Vector2 _fieldPosition;
    private Vector2 _fieldScale;
    private float _startCameraOrthographicSize;
    private Vector2 _startCameraPosition;
    private PositionScaleProperty _propertyColumn;
    private PositionScaleProperty _propertyRow;

    private void Awake()
    {
        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _halfSize = _gridSettings.CellSize / 2;
        _sizeWithSpace = _gridSettings.CellSize + _gridSettings.CellSpacing;
        _propertyRow = new PositionScaleProperty(0, _halfSize, 0, _gridSettings.CellSize);
        _propertyColumn = new PositionScaleProperty(_halfSize, 0 , _gridSettings.CellSize, 0);
    }

    public void UpdateWave()
    {
        if (++_currentWave == _fieldExpanderSettings.WaveCount && (_count > 0 || _fieldExpanderSettings.IsLoop))
        {
            _currentWave = 0;
            AddColumn();
        }
    }

    public void StartLevel()
    {
        _ballWaveVolume.Bag.Added += OnAbilityAdd;
        _fieldEffect.transform.position = _fieldPosition;
        _fieldShape.scale = _fieldScale;
        _currentWave = 0;
        _count = _fieldExpanderSettings.Count;
        _extraColumns = _gridSettings.Size.x;
        _extraRows = _gridSettings.Size.y;
        _cameras.SetGameplayBoardSize(BoardSize());
    }

    public void FinishLevel()
    {
        _ballWaveVolume.Bag.Added -= OnAbilityAdd;
        SetDefault();
    }

    private void OnAbilityAdd(BallVolumesBagCell volume)
    {
        if (volume.IsEqual(BallVolumesTypes.FieldExpander))
            AddRow();
    }
    
    private void AddColumn()
    {
        _count--;
        _gridSettings.AddSize(Vector2Int.right);
        _physicGrid.SpawnColumn(false, ++_extraColumns);
        SetComponentsPosition(_propertyColumn);
        _boards.ChangePositionScale(true, _propertyColumn);
    }

    private void AddRow()
    {
        _gridSettings.AddSize(Vector2Int.up);
        _physicGrid.SpawnRow(++_extraRows);
        _blocksBinder.StartMoveAllBlocks(Vector2Int.up, () => { return; });
        SetComponentsPosition(_propertyRow);
        _boards.ChangePositionScale(false, _propertyRow);
    }

    private void SetComponentsPosition(PositionScaleProperty property)
    {
        _fieldEffect.transform.position += property.Position;
        _fieldShape.scale += property.Scale;
        _cameras.AddValue(_cameras.Gameplay, position : property.Position);
        _cameras.SetGameplayBoardSize(BoardSize());
    }

    private Vector2 BoardSize()
    {
        return new Vector2(_sizeWithSpace * _extraColumns + FrameSize, _sizeWithSpace * _extraRows + FrameSize);
    }

    private void SetDefault()
    {
        _fieldEffect.transform.position = _fieldPosition;
        _fieldShape.scale = _fieldScale;
        _cameras.SetDefault();
        _currentWave = 0;
        _count = _fieldExpanderSettings.Count;
        _extraColumns = _gridSettings.Size.x;
        _extraRows = _gridSettings.Size.y;
    }

    public IDictionary<string, float> GetSavingData()
    {
        return new Dictionary<string, float>()
        {
            {FieldEffectPositionX,  _extraColumns}
        };
    }

    public void Load(IDictionary<string, float> data)
    {
        _extraColumns = Mathf.RoundToInt(data[FieldEffectPositionX]);
    }
}