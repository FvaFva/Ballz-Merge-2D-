using UnityEngine;
using System.Collections.Generic;
using System;
using Zenject;
using BallzMerge.Gameplay.BlockSpace;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, ILevelStarter, ISaveDependedObject, IDependentLevelSetting, IValueViewScore
    {
        private const string PointsToDrop = "PointsToDrop";
        private const string DropsInStuck = "DropsInStuck";

        [SerializeField] private int _wavePoints;
        [SerializeField] private int _mergePoints;
        [SerializeField] private int _toDrop;
        [SerializeField] private DropSelector _selector;

        [Inject] private BlocksInGame _blocks;

        private DropSettings _drop;
        private int _current;
        private int _dropInStuck;
        private (Drop, Drop) _drops = default;

        public bool IsReadyToDrop => _dropInStuck > 0;

        public event Action<IValueViewScore, int, int> ScoreChanged;

        private void OnEnable()
        {
            _blocks.BlocksMerged += OnMerge;
        }

        private void OnDisable()
        {
            _blocks.BlocksMerged -= OnMerge;
        }

        public void StartLevel(bool isAfterLoad)
        {
            if (isAfterLoad == false)
            {
                _current = 0;
                AddPoints();
            }

            ScoreChanged?.Invoke(this, _current, _toDrop);
        }

        public void Save(SaveDataContainer save)
        {
            foreach (IBallVolumesBagCell<BallVolume> bagCell in _selector.DropsMap)
                save.Volumes.Add(new SavedVolume(bagCell.ID, bagCell.Name, bagCell.Value));

            save.Set(PointsToDrop, _current);
            save.Set(DropsInStuck, _dropInStuck);
        }

        public void Load(SaveDataContainer save)
        {
            _current = Mathf.RoundToInt(save.Get(PointsToDrop));
            _dropInStuck = Mathf.RoundToInt(save.Get(DropsInStuck));
            ScoreChanged?.Invoke(this, _current, _toDrop);

            List<Drop> temp = _drop.GetPool();

            foreach (var savedVolume in save.Volumes)
            {
                for (int i = 0; i < temp.Count; i++)
                {
                    var drop = temp[i];

                    if (drop.Volume.Name == savedVolume.Name &&
                        drop.Rarity.Weight == savedVolume.Weight)
                    {
                        _selector.LoadDrop(new Drop(drop.Volume, drop.Rarity), savedVolume.ID);
                        break;
                    }
                }
            }
        }

        public void ShowDrop(Action callback)
        {
            if (IsReadyToDrop == false)
                return;

            _dropInStuck--;

            if (_drops.Item1.IsEmpty)
            {
                List<Drop> temp = _drop.GetPool();
                _drops = (temp.TakeRandom(), temp.TakeRandom());
            }

            _selector.Show(_drops.Item1, _drops.Item2, () => AfterTakeDrop(callback));
        }

        public void UpdateWave() => AddPoints(_wavePoints);

        public void ApplySettings(LevelSettings settings) => _drop = settings.DropSettings;

        private void OnMerge(Block _, Block __) => AddPoints(_mergePoints);

        private void AddPoints(int points = 0)
        {
            _current += points;

            while (_current >= _toDrop)
            {
                _current -= _toDrop;
                _dropInStuck++;
            }

            ScoreChanged?.Invoke(this, _current, _toDrop);
        }

        private void AfterTakeDrop(Action callback)
        {
            _drops = default;
            callback.Invoke();
        }
    }
}