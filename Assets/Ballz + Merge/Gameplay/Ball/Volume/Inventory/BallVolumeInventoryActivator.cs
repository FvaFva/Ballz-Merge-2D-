using BallzMerge.Gameplay.BallSpace;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BallVolumeInventoryActivator : MonoBehaviour
{
    [SerializeField] private BallVolumeCarrier _inventory;
    [SerializeField] private Button _trigger;

    [Inject] private UIRootView _rootView;
    [Inject] private Ball _ball;

    private bool _isShowing;
    private bool _forceShow;

    private void Awake()
    {
        _rootView.InfoPanelShowcase.UIViewStateChanged += ChangeStateInfoPanel;
    }

    private void OnEnable()
    {
        _trigger.AddListener(Activate);
        _ball.EnterAIM += ShowButton;
        _ball.LeftAIM += HideButton;
    }

    private void OnDisable()
    {
        _trigger.RemoveListener(Activate);
        _ball.EnterAIM -= ShowButton;
        _ball.LeftAIM -= HideButton;
    }

    private void OnDestroy()
    {
        _rootView.InfoPanelShowcase.UIViewStateChanged -= ChangeStateInfoPanel;
    }

    private void Activate()
    {
        _rootView.InfoPanelShowcase.Show(_inventory);
    }

    private void ChangeStateInfoPanel(bool state)
    {
        _forceShow = true;

        if (_isShowing)
            _trigger.SetActiveIfNotNull(state);
    }

    private void ShowButton()
    {
        _isShowing = true;

        if (!_forceShow)
            _trigger.SetActiveIfNotNull(true);
    }

    private void HideButton()
    {
        _forceShow = false;
        _isShowing = false;
        _trigger.SetActiveIfNotNull(false);
    }
}
