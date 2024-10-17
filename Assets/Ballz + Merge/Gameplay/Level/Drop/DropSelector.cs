using DG.Tweening;
using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, ILevelFinisher, IInitializable
    {
        private const float AnimationTime = 0.8f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

        private Action _callback;

        public event Action<BallVolumesTypes, float> DropSelected;

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
        }

        public void FinishLevel()
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        private void OnSelect(Drop drop, float count)
        {
            Hide();
            DropSelected?.Invoke(drop.WaveDropType, count);
        }

        private void Hide() => _canvasGroup.DOFade(0, AnimationTime).OnComplete(OnHideAnimationFinished);

        private void OnHideAnimationFinished()
        {
            _callback();
            _firstSlot.Show(null);
            _secondSlot.Show(null);
            gameObject.SetActive(false);
        }
    }
}