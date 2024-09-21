using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter, ILevelFinisher
{
    [SerializeField] private FieldExpanderSettings _fieldExpanderSettings;
    [SerializeField] private DropSelector _dropSelector;
    [SerializeField] private BoxCollider2D _leftBorder;
    [SerializeField] private BoxCollider2D _rightBorder;
    [SerializeField] private BoxCollider2D _topBorder;
    [SerializeField] private BoxCollider2D _bottomBorder;
    [SerializeField] private ParticleSystem _fieldEffect;
    [SerializeField] private Camera _camera;

    [Inject] private PhysicGrid _physicGrid;
    [Inject] private GridSettings _gridSettings;
    [Inject] private BlocksBus _blocksBus;

    private Dictionary<BoxCollider2D, StartProperty> _collidersProperty;
    private int _count;
    private int _extraColumns;
    private int _extraRows;
    private int _currentWave;

    private ParticleSystem.ShapeModule _fieldShape;
    private Vector3 _fieldPosition;
    private Vector3 _fieldScale;
    private float _cameraOrthographicSize;
    private Vector3 _cameraPosition;

    private void Awake()
    {
        _collidersProperty = new()
        {
            { _leftBorder, new StartProperty(_leftBorder.offset, _leftBorder.size) },
            { _rightBorder, new StartProperty(_rightBorder.offset, _rightBorder.size) },
            { _topBorder, new StartProperty(_topBorder.offset, _topBorder.size) },
            { _bottomBorder, new StartProperty(_bottomBorder.offset, _bottomBorder.size) }
        };

        _fieldShape = _fieldEffect.shape;
        _fieldPosition = _fieldEffect.transform.position;
        _fieldScale = _fieldEffect.shape.scale;
        _cameraOrthographicSize = _camera.orthographicSize;
        _cameraPosition = _camera.transform.position;
    }

    public void UpdateWave()
    {
        AddColumn(++_currentWave);
    }

    public void StartLevel()
    {
        _dropSelector.DropSelected += AddRow;

        foreach (var border in _collidersProperty)
        {
            border.Key.offset = border.Value.ColliderOffset;
            border.Key.size = border.Value.ColliderSize;
        }

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
        _dropSelector.DropSelected -= AddRow;
    }

    private void AddColumn(int currentWave)
    {
        if (currentWave % _fieldExpanderSettings.WaveCount == 0 && (_count > 0 || _fieldExpanderSettings.IsLoop))
        {
            _count--;
            _gridSettings.AddGridSize(Vector2Int.right);
            _physicGrid.SpawnColumn(false, ++_extraColumns);
            SetComponentsPosition(_rightBorder, _topBorder, _bottomBorder, new Vector3(_gridSettings.CellSize / 2, 0, 0), new Vector3(_gridSettings.CellSize, 0, 0));
        }
    }

    private void SetComponentsPosition(BoxCollider2D offsetBorder, BoxCollider2D firstSizeOffsetBorder, BoxCollider2D secondSizeOffsetBorder, Vector3 newPosition, Vector3 newScale)
    {
        offsetBorder.offset += (Vector2)newScale;

        ChangeSizeOffsetBorder(firstSizeOffsetBorder, newPosition, newScale);
        ChangeSizeOffsetBorder(secondSizeOffsetBorder, newPosition, newScale);

        _fieldEffect.transform.position += newPosition;
        _fieldShape.scale += newScale;
        _camera.orthographicSize += _gridSettings.CellSize / 2;
        _camera.transform.position += newPosition;
    }

    private void ChangeSizeOffsetBorder(BoxCollider2D border, Vector2 newPosition, Vector2 newScale)
    {
        border.offset += newPosition;
        border.size += newScale;
    }

    private void AddRow(BallVolumesTypes type, float count)
    {
        if (type == BallVolumesTypes.FieldExpander)
        {
            _gridSettings.AddGridSize(Vector2Int.up);
            _physicGrid.SpawnRow(++_extraRows);
            _blocksBus.MoveAllBlocks(Vector2Int.up);
            SetComponentsPosition(_topBorder, _leftBorder, _rightBorder, new Vector3(0, _gridSettings.CellSize / 2, 0), new Vector3(0, _gridSettings.CellSize, 0));
        }
    }
}
