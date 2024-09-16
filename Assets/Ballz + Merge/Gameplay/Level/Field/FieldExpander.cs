using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

public class FieldExpander : CyclicBehavior, IWaveUpdater, ILevelStarter
{
    [SerializeField] private FieldExpanderSettings _fieldExpanderSettings;

    [Inject] private PhysicGrid _physicGrid;
    [Inject] private GridSettings _gridSettings;

    private int _count;
    private int _extraColumn;
    private int _currentWave;

    private void Awake()
    {
        _count = _fieldExpanderSettings.Count;
        _extraColumn = _gridSettings.GridSize.x;
    }

    private void AddColumn(int currentWave)
    {
        if (currentWave % _fieldExpanderSettings.WaveCount == 0 && (_count > 0 || _fieldExpanderSettings.IsLoop))
        {
            _count--;
            _gridSettings.AddGridSize(Vector2Int.right);
            _physicGrid.SpawnColumn(++_extraColumn);
        }
    }

    public void UpdateWave()
    {
        AddColumn(++_currentWave);
    }

    public void StartLevel()
    {
        _currentWave = 0;
    }
}
