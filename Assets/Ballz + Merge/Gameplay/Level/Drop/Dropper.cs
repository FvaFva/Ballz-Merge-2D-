using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, IInitializable, ILevelLoader
    {
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
            foreach (BallVolume volume in _rarities)
            {
                for (int i = 0; i < rarity.CountInPool; i++)
                    _pool.Add(new Drop(volume, rarity));
            }
        }
    }
}