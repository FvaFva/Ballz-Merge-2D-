using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallWaveVolumeView : CyclicBehavior, IDependentScreenOrientation, IInitializable
{
    [SerializeField] private bool _isShowPassive;
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private GameDataVolumeMicView _viewPrefab;

    private Queue<GameDataVolumeMicView> _free = new Queue<GameDataVolumeMicView>();
    private Queue<GameDataVolumeMicView> _busy = new Queue<GameDataVolumeMicView>();
    private Action _update = () => { };
    private Func<IEnumerable<IBallVolumesBagCell<BallVolume>>> _getter = () => { return default; };
    private GameDataVolumeMicView _currentView;

    public IBallVolumesBagCell<BallVolume> CurrentData {  get; private set; }

    public event Action<bool> ActiveVolumePerformed;

    private void OnEnable()
    {
        _update();
        _source.Changed += ShowVolumes;
        _viewPort.sizeDelta = Vector2.zero;
    }

    private void OnDisable()
    {
        _source.Changed -= ShowVolumes;
        HideAll();
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

        if (_isShowPassive)
            _getter = () => _source.Bag.All;
        else
            _getter = () => _source.Bag.Hit;

        _update();
    }

    public void UpdateScreenOrientation(bool isVertical)
    {
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
                _currentView = null;
                ActiveVolumePerformed?.Invoke(false);
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

        ActiveVolumePerformed?.Invoke(gonnaActive);
    }

    private bool IsCurrentGonnaActive()
    {
        return _currentView != null && _currentView.IsActive && _currentView.Data.IsEqual<BallVolumeOnHit>();
    }
}