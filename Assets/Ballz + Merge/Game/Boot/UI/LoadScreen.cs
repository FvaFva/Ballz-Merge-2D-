using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;

namespace BallzMerge.Root
{
    public class LoadScreen : DependentColorUI
    {
        private const int MAX_DOTS = 3;

        [SerializeField] private GameRulesView _rulesView;
        [SerializeField] private BlockerToClick _blocker;
        [SerializeField] private Slider _progress;
        [SerializeField] private List<DependentColorUI> _dependentColorUIs;
        [SerializeField] private TMP_Text _title;

        private Action _showRules = () => { };
        private Action _hideRules = () => { };
        private Action _callBack;
        private int _countDots;

        public override void ApplyColors(GameColors gameColors)
        {
            foreach(var dependentColorUI in _dependentColorUIs)
                dependentColorUI.ApplyColors(gameColors);
        }

        public void Show()
        {
            _progress.value = 0;
            _showRules();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _hideRules();
            gameObject.SetActive(false);
        }

        public void ReadyToShowRule()
        {
            _showRules = () =>
            {
                _rulesView.ShowRule();
            };

            _hideRules = () =>
            {
                _rulesView.Hide();
            };
        }

        public void MoveProgress(float target, float step)
        {
            _progress.value = Mathf.Lerp(_progress.value, target, step);
            _countDots = (_countDots + 1) % MAX_DOTS;
            
            if(_progress.value.Equals(_progress.maxValue))
                _title.text = "Loaded: Tap to continue";
            else
                _title.text = $"Loading.{new string('.', _countDots)}";
        }

        public void MoveProgress(float target)
        {
            _progress.value = target;
        }

        public void WaitToClick(Action callback)
        {
            _callBack = callback;
            _blocker.Clicked += OnBlockerClicked;
            _blocker.Activate();
        }

        private void OnBlockerClicked()
        {
            _blocker.Clicked -= OnBlockerClicked;
            Hide();
            _callBack?.Invoke();
        }
    }
}
