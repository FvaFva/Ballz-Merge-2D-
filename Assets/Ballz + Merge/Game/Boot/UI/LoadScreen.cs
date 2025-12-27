using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace BallzMerge.Root
{
    public class LoadScreen : DependentColorUI
    {
        [SerializeField] private GameRulesView _rulesView;
        [SerializeField] private InfoPanelShowcase _showcase;
        [SerializeField] private Slider _progress;
        [SerializeField] private List<DependentColorUI> _dependentColorUIs;

        private Action _showRules = () => { };
        private Action _hideRules = () => { };

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
                _showcase.Close();
                _rulesView.ShowRule();
            };

            _hideRules = () =>
            {
                _showcase.Close();
            };
        }

        public void MoveProgress(float target, float step)
        {
            _progress.value = Mathf.Lerp(_progress.value, target, step);
        }

        public void MoveProgress(float target)
        {
            _progress.value = target;
        }
    }
}
