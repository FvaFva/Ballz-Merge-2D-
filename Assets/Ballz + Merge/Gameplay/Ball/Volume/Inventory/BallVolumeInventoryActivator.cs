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

    private void Activate() => _rootView.InfoPanelShowcase.Show(_inventory);

    private void ShowButton() => _trigger.SetActiveIfNotNull(true);
    private void HideButton() => _trigger.SetActiveIfNotNull(false);
}
