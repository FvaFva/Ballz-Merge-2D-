using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BlocksSpawner : CyclicBehaviour, ILevelStarter
{
    [SerializeField] private int _countPreload;
    [SerializeField] private Transform _blockParent;
    [SerializeField] private MoveSettingsNumbers _numbersProperties;
    [SerializeField] private MoveSettingsCountBlocks _countProperties;
    [SerializeField] private MoveColorMap _colorMap;

    [Inject] private DiContainer _container;
    [Inject] private Block _prefab;
    [Inject] private GridSettings _gridSettings;

    private Queue<Block> _blocks = new Queue<Block>();
    private List<Block> _allBlocks = new List<Block>();
    private int _currentWave;

    public override void Init()
    {
        for(int i  = 0; i < _countPreload; i++)
            _blocks.Enqueue(Generate());
    }

    public void StartLevel()
    {
        _currentWave = 0;
    }

    public IEnumerable<Block> SpawnWave()
    {
        _currentWave++;
        Block[] blocks = new Block[GetBlockCount()];
        List<int> positions = _gridSettings.GetPositionsInRow();

        for (int i = 0; i < blocks.Length; i++)
        {
            if (_blocks.TryDequeue(out blocks[i]) == false)
                blocks[i] = Generate();

            blocks[i].Deactivated += Deactivated;
            int number = GetBlockNumber();
            Vector2Int gridPosition = new Vector2Int(positions.TakeRandom(), _gridSettings.FirstRowIndex);
            blocks[i].Activate(number, gridPosition, _colorMap.GetColor(number));
        }

        return blocks;
    }

    private void Deactivated(Block block)
    {
        _blocks.Enqueue(block);
        block.Deactivated -= Deactivated;
    }

    private int GetBlockCount()
    {
        var property = _countProperties.Properties.Where(prop => prop.Range.IsInRange(_currentWave)).FirstOrDefault();

        if (property.IsEmpty())
            property = _countProperties.Properties.Where(item => item.Range.TerminalWave == _countProperties.Properties.Max(item => item.Range.TerminalWave)).FirstOrDefault();

        if (property.IsEmpty())
            Debug.Log("No property");

        int point = Random.Range(1, 101);
        int previous = 0;

        foreach(var chancesToCount in property.CountBlocks)
        {
            if (chancesToCount.Chance + previous > point)
                return chancesToCount.Count;

            previous += chancesToCount.Chance;
        }

        return 0;
    }

    private int GetBlockNumber()
    {
        var property = _numbersProperties.Properties.Where(prop => prop.Range.IsInRange(_currentWave)).FirstOrDefault();

        if (property.IsEmpty())
            property = _numbersProperties.Properties.Where(item => item.Range.TerminalWave == _numbersProperties.Properties.Max(item => item.Range.TerminalWave)).FirstOrDefault();

        if (property.IsEmpty())
            return 0;

        return property.NumbersToSpawn[Random.Range(0, property.NumbersToSpawn.Length)];
    }

    private Block Generate()
    {
        Block block = _container.InstantiatePrefabForComponent<Block>(_prefab).Initialize(_blockParent);
        _allBlocks.Add(block);
        return block;
    }
}