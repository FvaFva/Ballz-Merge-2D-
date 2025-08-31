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
    [SerializeField] private InfoPanelContent _content;
    [SerializeField] private EscapeMenu _escapeMenu;
    [SerializeField] private InfoPanelView _settingsPanel;

    [Inject] private MainInputMap _userInput;

    private Queue<IInfoPanelView> _panels = new Queue<IInfoPanelView>();
    private IInfoPanelView _current;

    public event Action<bool> UIViewStateChanged;
    public event Action CloseTriggered;

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

    public void Close()
    {
        if (TryActivate(_default))
            return;

        if (_panels.TryDequeue(out IInfoPanelView temp))
        {
            _current.Hide();
            _current = null;
            CloseTriggered?.Invoke();
            ShowPanel(temp);
        }
        else
        {
            Deactivate();
        }
    }

    private void OnCloseClick()
    {
        Close();
    }

    private void OpenDefault()
    {
        TryActivate(_default);
    }

    private void Deactivate()
    {
        _content.Close(OnContentClosed);
    }

    private void OnContentClosed()
    {
        UIViewStateChanged?.Invoke(true);
        HideAllPanels();
        _current = null;
        _panels.Clear();
        _openDefaultButton.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(false);
        _closeArea.gameObject.SetActive(false);
        CloseTriggered?.Invoke();
    }

    private bool TryActivate(IInfoPanelView panelView)
    {
        if (_current == null)
        {
            _content.Activate();
            ShowPanel(panelView);
            _content.Open();
            _openDefaultButton.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(true);
            _closeArea.gameObject.SetActive(true);
            UIViewStateChanged?.Invoke(false);
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
        _current?.Hide();

        if (_panels.IsEmpty() == false)
        {
            int panelsCount = _panels.Count;

            for (int i = 0; i < panelsCount; i++)
                _panels.Dequeue().Hide();
        }
    }
}
