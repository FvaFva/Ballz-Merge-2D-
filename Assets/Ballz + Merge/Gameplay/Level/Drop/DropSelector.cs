using BallzMerge.Data;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, IInitializable, ILevelSaver, ILevelLoader, ILevelFinisher
    {
        private const string CountOfSavedVolumes = "CountOfVolumes";
        private const float AnimationTime = 0.8f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

        [Inject] private DataBaseSource _data;

        private List<BallVolumesBagCell> _dropsMap = new List<BallVolumesBagCell>();
        private Action _callback;

        public IReadOnlyList<BallVolumesBagCell> DropsMap => _dropsMap;

        public event Action<BallVolumesBagCell> DropSelected;
        public event Action<BallVolumesBagCell> DropLoaded;
        public event Action Opened;

        private void OnEnable()
        {
            _firstSlot.Selected += OnSelect;
            _secondSlot.Selected += OnSelect;
        }

        private void OnDisable()
        {
            _firstSlot.Selected -= OnSelect;
            _secondSlot.Selected -= OnSelect;
        }

        public void Init()
        {
            _firstSlot.CashMaterials();
            _secondSlot.CashMaterials();
        }

        public void StartLevel()
        {
            _dropsMap.Clear();
        }

        public void GetSavingData()
        {
            _data.Saves.Save(new KeyValuePair<string, float>(CountOfSavedVolumes, _dropsMap.Count));
        }

        public void Show(Drop first, Drop second, Action callback)
        {
            _firstSlot.Show(first);
            _secondSlot.Show(second);
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, AnimationTime);
            _callback = callback;
            Opened?.Invoke();
        }

        public void FinishLevel()
        {
            _canvasGroup.alpha = 0;
            _dropsMap.Clear();
            gameObject.SetActive(false);
        }

        public void LoadDrop(Drop drop, int? id = null)
        {
            SelectDrop(drop, DropLoaded, id);
        }

        private void SelectDrop(Drop drop, Action<BallVolumesBagCell> action, int? id = null)
        {
            BallVolumesBagCell newCell = new BallVolumesBagCell(drop.Volume, drop.Rarity, id);
            action?.Invoke(newCell);
            _dropsMap.Add(newCell);
        }

        private void OnSelect(Drop drop)
        {
            Hide();
            SelectDrop(drop, DropSelected);
        }

        private void Hide() => _canvasGroup.DOFade(0, AnimationTime).OnComplete(OnHideAnimationFinished);

        private void OnHideAnimationFinished()
        {
            _firstSlot.Show(default);
            _secondSlot.Show(default);
            _callback();
            gameObject.SetActive(false);
        }
    }
}