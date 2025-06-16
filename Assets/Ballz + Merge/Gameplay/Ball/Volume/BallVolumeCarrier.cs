using System;
using BallzMerge.Root;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class BallVolumeCarrier : CyclicBehavior, IDropHandler, IInfoPanelView
{
    private const float AnimationTime = 0.15f;
    private const string ToBagHeader = "Drop here for bag";
    private const string ToCageHeader = "Click here for cage";

    [Header("Logic")] 
    [SerializeField] private UIRootContainerItem _containerItem;
    [SerializeField] private BallWaveVolume _volumes;
    [SerializeField] private BallWaveVolumeView _volumesView;
    [SerializeField] private BallVolumesCageView _cage;
    [SerializeField] private BallVolumeCageContainer _container;
    [SerializeField] private Button _trigger;
    [Header("Visual")]
    [SerializeField] private AnimatedButton _button;
    [SerializeField] private RectPumper _pumper;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private Image _headerParent;
    [SerializeField] private Image _lock;
    [SerializeField] private BallVolumeView _volumeView;

    [Inject] private UIRootView  _rootView;

    private RectTransform _transform;
    private bool _isOpen;
    private Tween _lockAnimation;
    private float _lockBaseFade;
    private Action<BallVolumesBagCell> _bagConnector = (BallVolumesBagCell ability) => {};

    private void Awake()
    {
        _lockBaseFade = _lock.color.a;
        _transform = (RectTransform)transform;
        _bagConnector = (BallVolumesBagCell ability) =>
        {
            ability.SetID(0);
            _volumes.Bag.ApplyVolume(ability);
        };
    }

    private void OnEnable()
    {
        _trigger.AddListener(OnTrigger);
        _container.Changed += OnContainerActivate;
        _volumesView.ActiveVolumePerformed += OnBagSpellActivate;
        ChangeActive(false);
    }

    private void OnDisable()
    {
        _trigger.RemoveListener(OnTrigger);
        _container.Changed -= OnContainerActivate;
        _volumesView.ActiveVolumePerformed -= OnBagSpellActivate;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        _container.Swap(default, _bagConnector);
    }

    public void Show(RectTransform showcase)
    {
        _transform.SetParent(showcase);
        _transform.anchorMin = new Vector2(0.15f, 0.15f);
        _transform.anchorMax = new Vector2(0.85f, 0.85f);
        _transform.anchoredPosition = Vector2.zero;
        _transform.sizeDelta = Vector2.zero;
    }

    public void Hide()
    {
        _transform.SetParent(null);
        StartCoroutine(_containerItem.UpdatePosition());
        _volumesView.HidePerformed();
        _isOpen = false;
        ChangeActive(false);
    }

    private void OnTrigger()
    {
        var ability = _volumesView.CurrentData;
        _cage.AddVolume(ability);
        _volumes.Bag.DropVolume(ability);
    }

    private void OnContainerActivate(bool state)
    {
        _header.text = ToBagHeader;
        _volumesView.HidePerformed();
        ChangeActive(state);
    }

    private void OnBagSpellActivate(bool state)
    {
        _header.text = ToCageHeader;
        ChangeActive(state);

        if (state)
            _volumeView.Show(_volumesView.CurrentData);
    }

    private void ChangeActive(bool state)
    {
        StopAnimations();
        _pumper.enabled = state;
        _button.enabled = state;
        _trigger.enabled = state;
        _volumeView.Deactivate();
        _headerParent.gameObject.SetActive(state);

        if (_isOpen != state)
        {
            _isOpen = state;

            if (state)
                _rootView.InfoPanelShowcase.Show(this);
            else
                _rootView.InfoPanelShowcase.Close();
        }

        float targetFade = state ? 0 : _lockBaseFade;
        float targetScale = state ? 2 : 1;
        _lockAnimation = DOTween.Sequence()
            .Join(_lock.DOFade(targetFade, AnimationTime))
            .Join(_lock.transform.DOScale(targetScale, AnimationTime).SetEase(Ease.InOutBounce));
    }

    private void StopAnimations()
    {
        if(_lockAnimation != null && _lockAnimation.IsActive())
            _lockAnimation.Kill();
    }
}
