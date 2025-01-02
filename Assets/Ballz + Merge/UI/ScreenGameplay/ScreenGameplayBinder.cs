using UnityEngine;
using UnityEngine.UI;

public class ScreenGameplayBinder : WindowBinder<ScreenGameplayViewModel> 
{
    [SerializeField] private Button _popupScoreButton;
    [SerializeField] private Button _popupValuesButton;

    private void OnEnable()
    {
        _popupScoreButton?.onClick.AddListener(OnPopupScoreButtonClicked);
        _popupValuesButton?.onClick.AddListener(OnPopupValuesButtonClicked);
    }

    private void OnDisable()
    {
        _popupScoreButton?.onClick.RemoveListener(OnPopupScoreButtonClicked);
        _popupValuesButton?.onClick.RemoveListener(OnPopupValuesButtonClicked);
    }

    private void OnPopupScoreButtonClicked()
    {
        ViewModel.RequestOpenScore();
    }

    private void OnPopupValuesButtonClicked()
    {
        ViewModel.RequestOpenValues();
    }
}
