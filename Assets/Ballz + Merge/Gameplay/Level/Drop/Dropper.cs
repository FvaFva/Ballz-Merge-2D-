using UnityEngine;
using System.Collections.Generic;
using System;
using BallzMerge.Data;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, ILevelSaver, ILevelLoader, IDependentSettings
    {
        private const string WavesToDrop = "WavesToDrop";

        [Inject] private DataBaseSource _data;

        [SerializeField] private int _wavesToDrop;
        [SerializeField] private DropSelector _selector;
        [SerializeField] private ValueView _view;

        private DropSettings _drop;
        private int _waveCount;

        public bool IsReadyToDrop { get; private set; }

        public void StartLevel()
        {
            _waveCount = _wavesToDrop;
            _view.Show(_waveCount);
        }

        public void GetSavingData()
        {
            List<SavedVolume> savedVolumes = new List<SavedVolume>();

            foreach (BallVolumesBagCell<BallVolume> bagCell in _selector.DropsMap)
                savedVolumes.Add(new SavedVolume(bagCell.ID, bagCell.Name, bagCell.Rarity.Weight));

            _data.Saves.Save(new KeyValuePair<string, float>(WavesToDrop, _waveCount));
            _data.Saves.SaveVolumes(savedVolumes);
        }

        public void Load()
        {
            _waveCount = Mathf.RoundToInt(_data.Saves.Get(WavesToDrop));
            _view.Show(_waveCount);

            IEnumerable<SavedVolume> savedVolumes = _data.Saves.GetSavedVolumes();
            List<Drop> temp = _drop.GetPool();

            foreach (var savedVolume in savedVolumes)
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

            IsReadyToDrop = false;
            List<Drop> temp = _drop.GetPool();
            _waveCount = _wavesToDrop;
            _view.Show(_waveCount);
            _selector.Show(temp.TakeRandom(), temp.TakeRandom(), callback);
        }

        public void UpdateWave()
        {
            IsReadyToDrop = --_waveCount <= 0;
            _view.Show(_waveCount);
        }

        public void ApplySettings(LevelSettings settings)
        {
            _drop = settings.DropSettings;
        }
    }
}