using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameRulesActivator : MonoBehaviour
{
    [SerializeField] private GameRulesView _view;
    [SerializeField] private Button _openButton;

    [Inject] private InfoPanelShowcase _infoPanelShowcase;

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
        _infoPanelShowcase.Show(_view);
    }
}
