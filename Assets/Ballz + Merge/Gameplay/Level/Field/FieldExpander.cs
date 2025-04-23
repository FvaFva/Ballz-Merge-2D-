using BallzMerge.Gameplay;
using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter, ILevelFinisher
{
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
    private PositionScaleProperty _propertyColumn;
    private PositionScaleProperty _propertyRow;

    private void Awake()
    {
        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _halfSize = _gridSettings.CellSize / 2;
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
    }

    public void FinishLevel()
    {
        _gridSettings.ReloadSize();
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
        _cameras.AddValue(_cameras.Gameplay, 0.2f, -property.Position);
    }
}
