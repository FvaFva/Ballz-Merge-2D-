using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter, ILevelFinisher
{
    [SerializeField] private PlayZoneBoards _boards;
    [SerializeField] private FieldExpanderSettings _fieldExpanderSettings;
    [SerializeField] private ParticleSystem _fieldEffect;
    [SerializeField] private Camera _camera;

    [Inject] private PhysicGrid _physicGrid;
    [Inject] private GridSettings _gridSettings;
    [Inject] private BlocksBus _blocksBus;
    [Inject] private BallWaveVolume _ballWaveVolume;

    private int _count;
    private int _extraColumns;
    private int _extraRows;
    private int _currentWave;

    private ParticleSystem.ShapeModule _fieldShape;
    private Vector2 _fieldPosition;
    private Vector2 _fieldScale;
    private float _cameraOrthographicSize;
    private Vector2 _cameraPosition;
    private PositionScaleProperty _propertyColumn;
    private PositionScaleProperty _propertyRow;

    private void Awake()
    {
        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _cameraOrthographicSize = _camera.orthographicSize;
        _cameraPosition = _camera.transform.position;
        _propertyRow = new PositionScaleProperty(new Vector2(0, _gridSettings.CellSize / 2), new Vector2(0, _gridSettings.CellSize));
        _propertyColumn = new PositionScaleProperty(new Vector2(_gridSettings.CellSize / 2, 0), new Vector2(_gridSettings.CellSize, 0));
    }

    public void UpdateWave()
    {
        AddColumn(++_currentWave);
    }

    public void StartLevel()
    {
        _ballWaveVolume.GlobalVolumes.ChangedVolume += AddRow;
        _fieldEffect.transform.position = _fieldPosition;
        _fieldShape.scale = _fieldScale;
        _camera.orthographicSize = _cameraOrthographicSize;
        _camera.transform.position = _cameraPosition;
        _currentWave = 0;
        _count = _fieldExpanderSettings.Count;
        _extraColumns = _gridSettings.GridSize.x;
        _extraRows = _gridSettings.GridSize.y;
    }

    public void FinishLevel()
    {
        _ballWaveVolume.GlobalVolumes.ChangedVolume -= AddRow;
    }

    private void AddColumn(int currentWave)
    {
        if (currentWave % _fieldExpanderSettings.WaveCount == 0 && (_count > 0 || _fieldExpanderSettings.IsLoop))
        {
            _count--;
            _gridSettings.AddGridSize(Vector2Int.right);
            _physicGrid.SpawnColumn(false, ++_extraColumns);
            SetComponentsPosition(_propertyColumn.Position, _propertyColumn.Scale);
            _boards.ChangePositionScale(true, _propertyColumn);
        }
    }

    private void AddRow(BallVolumesTypes type, float count)
    {
        if (type == BallVolumesTypes.FieldExpander)
        {
            _gridSettings.AddGridSize(Vector2Int.up);
            _physicGrid.SpawnRow(++_extraRows);
            _blocksBus.MoveAllBlocks(Vector2Int.up);
            SetComponentsPosition(_propertyRow.Position, _propertyRow.Scale);
            _boards.ChangePositionScale(false, _propertyRow);
        }
    }

    private void SetComponentsPosition(Vector3 newPosition, Vector3 newScale)
    {
        _fieldEffect.transform.position += newPosition;
        _fieldShape.scale += newScale;
        _camera.orthographicSize += _gridSettings.CellSize / 2;
        _camera.transform.position += newPosition;
    }
}
