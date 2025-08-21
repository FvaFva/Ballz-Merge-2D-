using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BallzMerge.MainMenu
{
    public class LevelContinueView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelVew;
        [SerializeField] private Button _continueGame;
        [SerializeField] private AnimatedButton _continueGameButtonView;

        public event Action Selected;

        private void OnEnable()
        {
            _continueGame.AddListener(OnActivate);
        }

        private void OnDisable()
        {
            _continueGame.RemoveListener(OnActivate);
        }

        public void ChangeState(bool newState, string levelView = "")
        {
            _levelVew.text = levelView;
            _levelVew.gameObject.SetActive(newState);
            _continueGame.interactable = newState;
            _continueGameButtonView.SetState(newState);
        }

        private void OnActivate() => Selected?.Invoke();
    }
}
