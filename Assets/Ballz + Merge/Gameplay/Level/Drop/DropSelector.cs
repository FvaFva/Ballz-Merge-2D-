using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, IInitializable, ILevelFinisher
    {
        private const float AnimationTime = 0.8f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

        private List<Drop> _dropsMap = new List<Drop>();
        private Action _callback;

        public IReadOnlyList<Drop> DropsMap => _dropsMap;

        public event Action<BallVolume, DropRarity> DropSelected;
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
            gameObject.SetActive(false);
        }

        public void SelectDrop(Drop drop)
        {
            DropSelected?.Invoke(drop.Volume, drop.Rarity);
            _dropsMap.Add(drop);
        }

        private void OnSelect(Drop drop)
        {
            Hide();
            SelectDrop(drop);
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