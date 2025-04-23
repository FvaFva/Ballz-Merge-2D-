using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter, ILevelFinisher, ILevelSaver
{
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

    private ParticleSystem.ShapeModule _fieldShape;
    private Vector2 _fieldPosition;
    private Vector2 _fieldScale;
    private float _startCameraOrthographicSize;
    private Vector2 _startCameraPosition;
    private PositionScaleProperty _propertyColumn;
    private PositionScaleProperty _propertyRow;

    public event Action<string, float> Saved;
    public event Action<ILevelSaver, string> Requested;

    private void Awake()
    {
        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _halfSize = _gridSettings.CellSize / 2;
        _propertyRow = new PositionScaleProperty(new Vector2(0, _halfSize), new Vector2(0, _gridSettings.CellSize));
        _propertyColumn = new PositionScaleProperty(new Vector2(_halfSize, 0), new Vector2(_gridSettings.CellSize, 0));
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
        SetDefault();
    }

    public void FinishLevel()
    {
        _ballWaveVolume.Bag.Added -= OnAbilityAdd;
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
        _cameras.AddValue(_cameras.Gameplay, 0.2f, property.Position);
        Save();
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

    public void Save()
    {
        Saved?.Invoke(FieldEffectPositionX, _fieldEffect.transform.position.x);
        Saved?.Invoke(FieldEffectPositionY, _fieldEffect.transform.position.y);
        Saved?.Invoke(FieldEffectScaleX, _fieldShape.scale.x);
        Saved?.Invoke(FieldEffectScaleY, _fieldShape.scale.y);
        Saved?.Invoke(CameraOrthographicSize, _cameras.Gameplay.orthographicSize);
        Saved?.Invoke(CameraPositionX, _cameras.Gameplay.transform.position.x);
        Saved?.Invoke(CameraPositionY, _cameras.Gameplay.transform.position.y);
    }

    public void Restore()
    {
        SetDefault();
        Save();
    }

    public void Request()
    {
        Requested?.Invoke(this, FieldEffectPositionX);
        Requested?.Invoke(this, FieldEffectPositionY);
        Requested?.Invoke(this, FieldEffectScaleX);
        Requested?.Invoke(this, FieldEffectScaleY);
        Requested?.Invoke(this, CameraOrthographicSize);
        Requested?.Invoke(this, CameraPositionX);
        Requested?.Invoke(this, CameraPositionY);
    }

    public void Load(string key, float value)
    {
        
    }
}