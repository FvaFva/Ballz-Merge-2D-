using BallzMerge.Root;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameRulesActivator : MonoBehaviour
{
    [SerializeField] private Button _openButton;

    [Inject] private UIRootView _rootView;

    private void OnEnable()
    {
        _openButton.AddListener(OnTrigger);
    }

    private void OnDisable()
    {
        _openButton.RemoveListener(OnTrigger);
    }

    private void OnTrigger()
    {
        _rootView.GameRulesView.ShowRule(0);
    }
}
