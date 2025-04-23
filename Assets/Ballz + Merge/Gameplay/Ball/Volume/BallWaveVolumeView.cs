using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallWaveVolumeView : CyclicBehavior, IDependentScreenOrientation
{
    [SerializeField] private bool _isLoadGlobalVolumesOnEnable;
    [SerializeField] private BallWaveVolume _source;
    [SerializeField] private RectTransform _viewPort;
    [SerializeField] private GameDataVolumeMicView _viewPrefab;
    [SerializeField] private ContentSizeFitter _fitter;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private AdaptiveLayoutGroup _layout;

    private Queue<GameDataVolumeMicView> _free = new Queue<GameDataVolumeMicView>();
    private Queue<GameDataVolumeMicView> _busy = new Queue<GameDataVolumeMicView>();

    private void OnEnable()
    {
        _source.Changed += OnSourceUpdate;

        if(_isLoadGlobalVolumesOnEnable)
            ShowVolumes(_source.Bag.All);
        else
            ShowVolumes(_source.GetActiveVolumes());
    }

    private void OnDisable()
    {
        _source.Changed -= OnSourceUpdate;
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

    private void OnSourceUpdate()
    {
        ShowVolumes(_source.GetActiveVolumes());
    }

    private void ShowVolumes(IEnumerable<BallVolumesBagCell> volumes)
    {
        HideAll();

        foreach (var newValue in _source.GetActiveVolumes())
        {
            GameDataVolumeMicView tempView;

            if (_free.TryDequeue(out tempView) == false)
                tempView = Instantiate(_viewPrefab, _viewPort).Init();

            tempView.Show(newValue.Volume, newValue.Value);
            _busy.Enqueue(tempView);
        }
    }

    private void HideAll()
    {
        while (_busy.TryDequeue(out GameDataVolumeMicView tempView))
            _free.Enqueue(tempView.Hide());
    }
}