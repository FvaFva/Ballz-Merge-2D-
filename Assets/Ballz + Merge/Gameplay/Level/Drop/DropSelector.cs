using DG.Tweening;
using System;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class DropSelector : CyclicBehavior, ILevelFinisher
    {
        private const float AnimationTime = 0.5f;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private DropView _firstSlot;
        [SerializeField] private DropView _secondSlot;

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

        public void Show(Drop first, Drop second)
        {
            _firstSlot.Show(first);
            _secondSlot.Show(second);
            _canvas.enabled = true;
            _canvasGroup.DOFade(1, AnimationTime);
        }

        public void FinishLevel()
        {
            _canvasGroup.alpha = 0;
            _canvas.enabled = false;
        }

        private void OnSelect(Drop drop, float count)
        {
            Hide();
            DropSelected?.Invoke(drop.WaveDropType, count);
        }

        private void Hide()
        {
            _firstSlot.Show(null);
            _secondSlot.Show(null);
            _canvasGroup.DOFade(0, AnimationTime).OnComplete(() => _canvas.enabled = false);
        }
    }
}