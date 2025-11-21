using System;
using System.Collections.Generic;
using BallzMerge.Root;
using BallzMerge.Root.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallVolumeCarrier : DependentColorUI, IInfoPanelView, IDependentScreenOrientation
{
    private const string ToBagHeader = "Drop here for bag";
    private const string ToCageHeader = "Click here for cage";

    [SerializeField] private BallVolumeCarrierOrientationAdapter _adapter;
    [SerializeField] private BallVolumesPassiveView _passiveView;
    [SerializeField] private BallWaveVolume _volumes;
    [SerializeField] private BallWaveVolumeView _volumesView;
    [SerializeField] private RectTransform _cagePosition;
    [SerializeField] private BallVolumesCageView _cage;
    [SerializeField] private BallVolumeCageContainer _cageContainer;
    [SerializeField] private UIRootContainerItem _cageContainerItem;
    [SerializeField] private Button _trigger;
    [SerializeField] private BallVolumeCarrierView _volumeView;
    [SerializeField] private DropHandlerProxy _dropHandler;
    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private List<BackgroundUI> _backgroundUIs;

    private RectTransform _transform;
    private Transform _baseParent;
    private Action<BallVolumesBagCell<BallVolumeOnHit>> _bagConnector = (BallVolumesBagCell<BallVolumeOnHit> ability) => { };

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _baseParent = _transform.parent;
        _bagConnector = (BallVolumesBagCell<BallVolumeOnHit> ability) =>
        {
            ability?.SetID(0);
            _volumes.Bag.ApplyVolume(ability);
        };
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _trigger.AddListener(OnTrigger);
        _cageContainer.Changed += OnContainerActivate;
        _volumesView.ActiveVolumePerformed += OnBagSpellActivate;
        _dropHandler.Dropped += OnDrop;
        _passiveView.VolumeActivated += OnPassiveActivate;
    }

    private void OnDisable()
    {
        _trigger.RemoveListener(OnTrigger);
        _cageContainer.Changed -= OnContainerActivate;
        _volumesView.ActiveVolumePerformed -= OnBagSpellActivate;
        _dropHandler.Dropped -= OnDrop;
        _passiveView.VolumeActivated -= OnPassiveActivate;
    }

    public override void ApplyColors(GameColors gameColors)
    {
        foreach (var backgroundUI in _backgroundUIs)
            backgroundUI.ApplyColors(gameColors);
    }

    public void OnDrop(PointerEventData _)
    {
        if (_cageContainer.Current != null)
            _cageContainer.Swap(default, _bagConnector);
    }

    public void Show(RectTransform showcase)
    {
        gameObject.SetActive(true);
        _transform.SetParent(showcase);
        _cageContainerItem.PuckUp(_adapter.CagePosition);
        _cage.UpdateCompellation(true, _adapter.CageSeparate);
        _transform.anchoredPosition = Vector2.zero;
        _transform.sizeDelta = Vector2.zero;
        _volumeView.ChangeActive(false);
    }

    public void Hide()
    {
        _transform.SetParent(_baseParent);
        _volumesView.HidePerformed();
        _volumeView.ChangeActive(false);
        _cageContainerItem.UpdatePositionByGroup();
        _cage.UpdateCompellation();
        gameObject.SetActive(false);
    }

    public void UpdateScreenOrientation(bool isVertical)
    {
        _adapter.UpdateScreenOrientation(isVertical);

        if(gameObject.activeSelf)
        {
            _cageContainerItem.PuckUp(_adapter.CagePosition);
            _cage.UpdateCompellation(true, _adapter.CageSeparate);
        }
    }

    private void OnTrigger()
    {
        var ability = _volumesView.CurrentData;
        _cage.AddVolume(ability as BallVolumesBagCell<BallVolumeOnHit>);
        _volumes.Bag.DropVolume(ability);
    }

    private void OnContainerActivate(bool state)
    {
        _volumesView.HidePerformed();
        _volumeView.ChangeActive(state, _cageContainer.Current, ToBagHeader);
    }

    private void OnBagSpellActivate(bool state)
    {
        _volumeView.ChangeActive(state, _volumesView.CurrentData, ToCageHeader);
        _audio?.Play(AudioEffectsTypes.Bloop);
    }

    private void OnPassiveActivate(IBallVolumeViewData data)
    {
        _volumesView.HidePerformed();
        _volumeView.ChangeActive(true, data);
        _audio?.Play(AudioEffectsTypes.Bloop);
    }
}
