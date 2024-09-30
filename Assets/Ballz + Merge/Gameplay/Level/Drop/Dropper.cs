using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace BallzMerge.Gameplay.Level
{
    public class Dropper : CyclicBehavior, IInitializable, ILevelStarter
    {
        [SerializeField] private int _wavesToDrop;
        [SerializeField] private DropSelector _selector;
        [SerializeField] private List<Drop> _drops;
        [SerializeField] private ValueView _view;

        private List<Drop> _pool;
        private int _waveCount;

        public bool IsReadyToDrop { get; private set; }

        public void Init()
        {
            _pool = new List<Drop>();

            foreach (Drop drop in _drops)
            {
                for (int i = 0; i < drop.CountInPool; i++)
                    _pool.Add(drop);
            }
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
    }
}