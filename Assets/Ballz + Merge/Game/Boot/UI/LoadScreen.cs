using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace BallzMerge.Root
{
    public class LoadScreen : DependentColorUI
    {
        [SerializeField] private Slider _progress;
        [SerializeField] private TMP_Text _hint;
        [SerializeField] private GameRulesList _rules;
        [SerializeField] private List<DependentColorUI> _dependentColorUIs;

        public override void ApplyColors(GameColors gameColors)
        {
            foreach(var dependentColorUI in _dependentColorUIs)
                dependentColorUI.ApplyColors(gameColors);
        }

        public void Show()
        {
            _progress.value = 0;
            gameObject.SetActive(true);
            _hint.text = _rules.Get();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
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
