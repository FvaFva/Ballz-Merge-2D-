using UnityEngine;
using System.Collections.Generic;
using System;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, ILevelStarter, ISaveDependedObject, IDependentSettings
    {
        private const string WavesToDrop = "WavesToDrop";

        [SerializeField] private int _wavesToDrop;
        [SerializeField] private DropSelector _selector;
        [SerializeField] private ValueView _view;

        private DropSettings _drop;
        private int _waveCount;

        public bool IsReadyToDrop { get; private set; }

        public void StartLevel(bool isAfterLoad)
        {
            if (isAfterLoad)
                return;

            _waveCount = _wavesToDrop;
            _view.Show(_waveCount);
        }

        public void Save(SaveDataContainer save)
        {
            foreach (IBallVolumesBagCell<BallVolume> bagCell in _selector.DropsMap)
                save.Volumes.Add(new SavedVolume(bagCell.ID, bagCell.Name, bagCell.Value));

            save.Set(WavesToDrop, _waveCount);
        }

        public void Load(SaveDataContainer save)
        {
            _waveCount = Mathf.RoundToInt(save.Get(WavesToDrop));
            _view.Show(_waveCount);

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