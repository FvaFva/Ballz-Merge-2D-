using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, IInitializable, ILevelSaver, ILevelLoader
    {
        private const string WavesToDrop = "WavesToDrop";
        private const string SavedVolumes = "SavedVolumes";

        [SerializeField] private int _wavesToDrop;
        [SerializeField] private DropSelector _selector;
        [SerializeField] private DropRarity _rar;
        [SerializeField] private DropRarity _legendary;
        [SerializeField] private DropRarity _common;
        [SerializeField] private List<BallVolume> _commons;
        [SerializeField] private List<BallVolume> _rarities;
        [SerializeField] private List<BallVolume> _legends;
        [SerializeField] private ValueView _view;

        private List<Drop> _pool;
        private int _waveCount;

        public bool IsReadyToDrop { get; private set; }

        public void Init()
        {
            _pool = new List<Drop>();

            InitRarity(_rar, _rarities);
            InitRarity(_common, _commons);
            InitRarity(_legendary, _legends);
        }

        public void StartLevel()
        {
            _waveCount = _wavesToDrop;
            _view.Show(_waveCount);
        }

        public IDictionary<string, object> GetSavingData()
        {
            List<SavedVolume> savedVolumes = new List<SavedVolume>();

            foreach (var bagCell in _selector.DropsMap)
                savedVolumes.Add(new SavedVolume(bagCell.ID, bagCell.Volume.Type.ToString(), bagCell.Volume.Species.ToString(), bagCell.Rarity.Weight));

            return new Dictionary<string, object>()
            {
                { SavedVolumes, savedVolumes },
                { WavesToDrop, _waveCount }
            };
        }

        public void Load(IDictionary<string, object> data)
        {
            _waveCount = JsonConvert.DeserializeObject<int>(data[WavesToDrop].ToString());
            _view.Show(_waveCount);

            List<SavedVolume> savedVolumes = JsonConvert.DeserializeObject<List<SavedVolume>>(data[SavedVolumes].ToString());

            foreach (var savedVolume in savedVolumes)
            {
                for (int i = 0; i < _pool.Count; i++)
                {
                    var drop = _pool[i];

                    if (drop.Volume.Type.ToString() == savedVolume.Name &&
                        drop.Rarity.Weight == savedVolume.Weight &&
                        drop.Volume.Species.ToString() == savedVolume.Species)
                    {
                        _selector.SelectDrop(new Drop(drop.Volume, drop.Rarity), savedVolume.ID);
                        break;
                    }
                }
            }
        }

        public void ShowDrop(Action callback)
        {
            if (IsReadyToDrop == false)
                return;

            IsReadyToDrop = false;
            List<Drop> temp = _pool.ToList();
            _waveCount = _wavesToDrop;
            _view.Show(_waveCount);
            _selector.Show(temp.TakeRandom(), temp.TakeRandom(), callback);
        }

        public void UpdateWave()
        {
            IsReadyToDrop = --_waveCount <= 0;
            _view.Show(_waveCount);
        }

        private void InitRarity(DropRarity rarity, List<BallVolume> volumes)
        {
            foreach (BallVolume volume in volumes)
            {
                for (int i = 0; i < rarity.CountInPool; i++)
                    _pool.Add(new Drop(volume, rarity));
            }
        }
    }
}