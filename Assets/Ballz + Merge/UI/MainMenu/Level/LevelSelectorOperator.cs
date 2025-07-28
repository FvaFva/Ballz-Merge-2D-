using System;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelSelectorOperator : MonoBehaviour
{
    [SerializeField] private LevelSelector _selector;
    [SerializeField] private Button _button;

    [Inject] private UIRootView _uiRoot;

    public event Action Selected;

    private void OnEnable()
    {
        _button.AddListener(OnTrigger);
        _selector.Selected += OnSelect;
    }

    private void OnDisable()
    {
        _button.RemoveListener(OnTrigger);
        _selector.Selected -= OnSelect;
    }

    private void OnTrigger()
    {
        _uiRoot.InfoPanelShowcase.Show(_selector);
    }

    private void OnSelect()
    {
        _uiRoot.InfoPanelShowcase.Close();
        Selected?.Invoke();
    }
}
