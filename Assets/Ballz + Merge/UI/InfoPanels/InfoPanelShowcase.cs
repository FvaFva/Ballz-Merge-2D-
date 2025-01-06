using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class InfoPanelShowcase : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _openDefaultButton;
    [SerializeField] private RectTransform _box;
    [SerializeField] private InfoPanelView _default;
    [SerializeField] private GameObject _content;

    [Inject] private MainInputMap _userInput;

    private Queue<IInfoPanelView> _panels = new Queue<IInfoPanelView>();
    private IInfoPanelView _current;

    private void OnEnable()
    {
        _closeButton.AddListener(OnCloseClick);
        _openDefaultButton.AddListener(OpenDefault);
        _userInput.MainInput.Escape.performed += OnEscape;
    }

    private void OnDisable()
    {
        _closeButton.RemoveListener(OnCloseClick);
        _openDefaultButton.RemoveListener(OpenDefault);
        _userInput.MainInput.Escape.performed += OnEscape;
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        OnCloseClick();
    }

    public void Show(IInfoPanelView panelView)
    {
        if(TryActivate(panelView) == false)
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
        _openDefaultButton.gameObject.SetActive(false);
    }

    private void Deactivate()
    {
        _current = null;
        _panels.Clear();
        _content.SetActive(false);
        _openDefaultButton.gameObject.SetActive(true);
    }

    private bool TryActivate(IInfoPanelView panelView)
    {
        if (_current == null)
        {
            ShowPanel(panelView);
            _content.SetActive(true);
            return true;
        }

        return false;
    }

    private void ShowPanel(IInfoPanelView panelView)
    {
        _current = panelView;
        _current.Show(_box);
    }
}
