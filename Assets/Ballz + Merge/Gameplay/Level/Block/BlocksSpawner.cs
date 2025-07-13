using BallzMerge.Data;
using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Gameplay.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlocksSpawner : CyclicBehavior, IInitializable, ILevelSaver, ILevelLoader
    {
        private const string CurrentWave = "CurrentWave";

        [SerializeField] private int _countPreload;
        [SerializeField] private Transform _blockParent;
        [SerializeField] private MoveSettingsCountBlocks _settings;
        [SerializeField] private MoveColorMap _colorMap;
        [SerializeField] private Block _prefab;
        [SerializeField] private VirtualWorldFactory _factory;

        [Inject] private DiContainer _container;
        [Inject] private DataBaseSource _data;
        [Inject] private GridSettings _gridSettings;
        [Inject] private BlocksInGame _activeBlocks;

        private Queue<Block> _blocks = new Queue<Block>();
        private int _currentWave;
        private int _blockID;

        private void OnEnable()
        {
            _activeBlocks.BlockRemoved += OnBlockFree;
        }

        private void OnDisable()
        {
            _activeBlocks.BlockRemoved -= OnBlockFree;
        }

        public void Init()
        {
            _blockID = 0;

            for (int i = 0; i < _countPreload; i++)
                _blocks.Enqueue(Generate(++_blockID));
        }

        public void StartLevel()
        {
            _currentWave = 0;
        }

        public void GetSavingData()
        {
            _data.Saves.Save(new KeyValuePair<string, float>(CurrentWave, _currentWave));
        }

        public void Load()
        {
            _currentWave = Mathf.RoundToInt(_data.Saves.Get(CurrentWave));
        }

        public IEnumerable<Block> SpawnWave()
        {
            _currentWave++;
            List<int> positions = _gridSettings.GetPositionsInRow();
            var current = GetCurrent();

            if (current.IsEmpty())
            {
                Vector2Int gridPosition = new Vector2Int(positions.TakeRandom(), _gridSettings.FirstRowIndex);
                yield return SpawnBlock(1, gridPosition);
            }
            else
            {
                int count = Mathf.Min(GetValue(current.Count), positions.Count);

                for (int i = 0; i < count; i++)
                {
                    int number = GetValue(current.Number);
                    Vector2Int gridPosition = new Vector2Int(positions.TakeRandom(), _gridSettings.FirstRowIndex);
                    yield return SpawnBlock(number, gridPosition);
                }
            }
        }

        public Block SpawnBlock(int number, Vector2Int gridPosition, int? id = null)
        {
            int blockID;

            if (_blocks.TryDequeue(out Block temp) == false)
            {
                blockID = id == null ? ++_blockID : (int)id;
                temp = Generate(blockID);
            }

            blockID = id == null ? temp.ID : (int)id;
            ActivateBlock(blockID, temp, number, gridPosition);
            return temp;
        }

        public void ResetBlocksID()
        {
            int newID = 0;

            foreach(Block block in _activeBlocks.Blocks)
                block.ChangeID(++newID);
        }

        private void ActivateBlock(int id, Block block, int number, Vector2Int gridPosition)
        {
            block.Activate(id, number, gridPosition, _colorMap.GetColor(number));
            _activeBlocks.AddBlocks(block);
        }

        private WaveSpawnProperty GetCurrent()
        {
            int set = Mathf.Min(_currentWave, _settings.Properties.Count() - 1);
            return _settings.Properties[set];
        }

        private int GetValue(IEnumerable<BlocksSpawnProperty> prop)
        {
            float point = Random.Range(0, prop.Sum(c => c.Weight));
            float previous = 0;

            foreach (var blockSpawnProperty in prop)
            {
                if (blockSpawnProperty.Weight + previous > point)
                    return blockSpawnProperty.Value;

                previous += blockSpawnProperty.Weight;
            }

            return 0;
        }

        private Block Generate(int id) => _container.InstantiatePrefabForComponent<Block>(_prefab).Initialize(id, _blockParent, _factory.GenerateBoxForBlock());

        private void OnBlockFree(Block block) => _blocks.Enqueue(block);
    }
}