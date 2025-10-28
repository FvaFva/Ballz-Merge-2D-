using System;
using TMPro;
using UnityEngine;

namespace BallzMerge.MainMenu
{
    public class LevelContinueView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelView;
        [SerializeField] private ButtonProperty _continueGame;

        public event Action Selected;

        private void OnEnable()
        {
            _continueGame.ChangeListeningState(OnActivate, true);
        }

        private void OnDisable()
        {
            _continueGame.ChangeListeningState(OnActivate, false);
        }

        public void ChangeState(bool newState, string levelView = "")
        {
            _levelView.text = levelView;
            _levelView.gameObject.SetActive(newState);
            _continueGame.ChangeState(newState);
        }

        private void OnActivate() => Selected?.Invoke();
    }
}
