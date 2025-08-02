using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, IInitializable, ILevelStarter, ILevelFinisher
    {
        private const float AnimationTime = 0.8f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

        private List<IBallVolumesBagCell<BallVolume>> _dropsMap = new List<IBallVolumesBagCell<BallVolume>>();
        private Action _callback;

        public IReadOnlyList<IBallVolumesBagCell<BallVolume>> DropsMap => _dropsMap;

        public event Action<IBallVolumesBagCell<BallVolume>> DropSelected;
        public event Action<IBallVolumesBagCell<BallVolume>> DropLoaded;
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
            _firstSlot.InitMaterial();
            _secondSlot.InitMaterial();
        }

        public void StartLevel(bool _)
        {
            _dropsMap.Clear();
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

        public void LoadDrop(Drop drop, int id)
        {
            SelectDrop(drop, DropLoaded, id);
        }

        private void SelectDrop(Drop drop, Action<IBallVolumesBagCell<BallVolume>> action, int? id = null)
        {
            IBallVolumesBagCell<BallVolume> newCell = null;
            
            if (drop.Volume is BallVolumePassive passive)
                newCell = new BallVolumesBagCell<BallVolumePassive>(passive, drop.Rarity, id);
            else if (drop.Volume is BallVolumeOnHit hit)
                newCell = new BallVolumesBagCell<BallVolumeOnHit>(hit, drop.Rarity, id);

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