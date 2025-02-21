using BallzMerge.Root.Settings;
using ModestTree;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class InfoPanelShowcase : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _closeArea;
    [SerializeField] private Button _openDefaultButton;
    [SerializeField] private RectTransform _box;
    [SerializeField] private InfoPanelView _default;
    [SerializeField] private GameObject _content;
    [SerializeField] private EscapeMenu _escapeMenu;
    [SerializeField] private InfoPanelView _settingsPanel;

    [Inject] private MainInputMap _userInput;

    private Queue<IInfoPanelView> _panels = new Queue<IInfoPanelView>();
    public event Action<bool> UIViewStateChanged;
    private IInfoPanelView _current;

    private void Start()
    {
        _closeArea.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _closeButton.AddListener(OnCloseClick);
        _closeArea.AddListener(Deactivate);
        _escapeMenu.CloseRequired += Deactivate;
        _escapeMenu.QuitRequired += OnQuitRequired;
        _escapeMenu.SettingsRequired += OpenSettings;
        _openDefaultButton.AddListener(OpenDefault);
        _userInput.MainInput.Escape.performed += OnEscape;
    }

    private void OnDisable()
    {
        _closeButton.RemoveListener(OnCloseClick);
        _closeArea.RemoveListener(Deactivate);
        _escapeMenu.CloseRequired -= Deactivate;
        _escapeMenu.QuitRequired -= OnQuitRequired;
        _escapeMenu.SettingsRequired -= OpenSettings;
        _openDefaultButton.RemoveListener(OpenDefault);
        _userInput.MainInput.Escape.performed += OnEscape;
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        OnCloseClick();
    }

    private void OnQuitRequired(SceneExitData sceneExitData)
    {
        Deactivate();
    }

    private void OpenSettings()
    {
        Show(_settingsPanel);
    }

    public void Show(IInfoPanelView panelView)
    {
        if (TryActivate(panelView) == false)
        {
            _panels.Enqueue(_current);
            _current.Hide();
            ShowPanel(panelView);
        }
    }

    private void OnCloseClick()
    {
        if (TryActivate(_default))
            return;
        else
            _current.Hide();

        if (_panels.TryDequeue(out IInfoPanelView temp))
            ShowPanel(temp);
        else
            Deactivate();
    }

    private void OpenDefault()
    {
        TryActivate(_default);
    }

    private void Deactivate()
    {
        UIViewStateChanged?.Invoke(true);
        HideAllPanels();
        _current = null;
        _panels.Clear();
        _content.SetActive(false);
        _openDefaultButton.gameObject.SetActive(true);
        _closeArea.gameObject.SetActive(false);
    }

    private bool TryActivate(IInfoPanelView panelView)
    {
        if (_current == null)
        {
            ShowPanel(panelView);
            UIViewStateChanged?.Invoke(false);
            _content.SetActive(true);
            _openDefaultButton.gameObject.SetActive(false);
            _closeArea.gameObject.SetActive(true);
            return true;
        }

        return false;
    }

    private void ShowPanel(IInfoPanelView panelView)
    {
        _current = panelView;
        _current.Show(_box);
    }

    private void HideAllPanels()
    {
        _current.Hide();

        if (_panels.IsEmpty() == false)
        {
            int panelsCount = _panels.Count;

            for (int i = 0; i < panelsCount; i++)
                _panels.Dequeue().Hide();
        }
    }
}
