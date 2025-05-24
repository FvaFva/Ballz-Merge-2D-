using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallWaveVolumeView : CyclicBehavior, IDependentScreenOrientation, IInitializable
{
    [SerializeField] private bool _showOnlyBag;
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private GameDataVolumeMicView _viewPrefab;
    [SerializeField] private ContentSizeFitter _fitter;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private AdaptiveLayoutGroup _layout;

    private Queue<GameDataVolumeMicView> _free = new Queue<GameDataVolumeMicView>();
    private Queue<GameDataVolumeMicView> _busy = new Queue<GameDataVolumeMicView>();
    private Action _update = () => { };
    private Func<IEnumerable<BallVolumesBagCell>> _getter = () => { return default; };
    private GameDataVolumeMicView _currentView;

    public BallVolumesBagCell CurrentData {  get; private set; }

    public event Action<bool> ActiveVolumePerformed;

    private void OnEnable()
    {
        _update();
        _source.Changed += ShowVolumes;

        foreach(GameDataVolumeMicView cell in _busy)
            cell.Performed += ChangeCurrent;
    }

    private void OnDisable()
    {
        _source.Changed -= ShowVolumes;

        foreach (GameDataVolumeMicView cell in _busy)
            cell.Performed -= ChangeCurrent;
    }

    public void HidePerformed()
    {
        if (_currentView == null)
            return;

        _currentView.Unperformed();
        _currentView = null;
    }

    public void Init()
    {
        _update = ShowVolumes;

        if (_showOnlyBag)
            _getter = () => _source.Bag.All;
        else
            _getter = () => _source.GetAllVolumes();

        _update();
    }

    public void UpdateScreenOrientation(ScreenOrientation orientation)
    {
        if (!_layout.IsInverse ^ (orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight))
        {
            _fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            _fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            _scroll.vertical = true;
            _scroll.horizontal = false;
        }
        else
        {
            _fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            _fitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            _scroll.vertical = false;
            _scroll.horizontal = true;
        }

        _viewPort.sizeDelta = Vector2.zero;
    }

    private void ShowVolumes()
    {
        HideAll();

        foreach (var newValue in _getter())
        {
            if (newValue == null)
                continue;

            if (_free.TryDequeue(out var tempView) == false)
                tempView = Instantiate(_viewPrefab, _viewPort).Init();

            tempView.Show(newValue);
            tempView.Performed += ChangeCurrent;
            _busy.Enqueue(tempView);
        }
    }

    private void HideAll()
    {
        ChangeCurrent(null);

        while (_busy.TryDequeue(out var tempView))
        {
            tempView.Performed -= ChangeCurrent;
            _free.Enqueue(tempView.Hide());
        }
    }

    private void ChangeCurrent(GameDataVolumeMicView view)
    {
        bool isHaveCurrent = _currentView != null;
        bool wasActive = false;

        if (isHaveCurrent)
        {
            if (_currentView == view)
            {
                ActiveVolumePerformed?.Invoke(_currentView.IsActive);
                return;
            }
            else
            {
                wasActive = IsCurrentGonnaActive();
                _currentView.Unperformed();
            }
        }
        
        _currentView = view;
        bool gonnaActive = IsCurrentGonnaActive();

        if (gonnaActive)
            CurrentData = _currentView.Data;
        else
            CurrentData = default;

        if (wasActive != gonnaActive)
            ActiveVolumePerformed?.Invoke(gonnaActive);
    }

    private bool IsCurrentGonnaActive()
    {
        return _currentView != null && _currentView.IsActive && _currentView.Data.Volume.Species == BallVolumesSpecies.Hit;
    }
}