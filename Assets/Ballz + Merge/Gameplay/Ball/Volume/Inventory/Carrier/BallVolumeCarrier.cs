using System;
using System.Collections;
using BallzMerge.Root;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallVolumeCarrier : CyclicBehavior, IInfoPanelView
{
    private const float ShowDuration = 0.15f;
    private const float BoardSize = 0.12f;
    private const string ToBagHeader = "Drop here for bag";
    private const string ToCageHeader = "Click here for cage";

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

    private RectTransform _transform;
    private Transform _baseParent;
    private Action<BallVolumesBagCell> _bagConnector = (BallVolumesBagCell ability) => { };

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _baseParent = _transform.parent;
        _bagConnector = (BallVolumesBagCell ability) =>
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

    public void OnDrop(PointerEventData _)
    {
        if (_cageContainer.Current != null)
            _cageContainer.Swap(default, _bagConnector);
    }

    public void Show(RectTransform showcase)
    {
        gameObject.SetActive(true);
        _transform.SetParent(showcase);
        _cageContainerItem.PuckUp(_cagePosition);
        _transform.anchorMin = new Vector2(BoardSize, BoardSize);
        _transform.anchorMax = new Vector2(1 - BoardSize, 1 - BoardSize);
        _transform.anchoredPosition = Vector2.zero;
        _transform.sizeDelta = Vector2.zero;
        _volumeView.ChangeActive(false);
    }

    public void Hide()
    {
        _transform.SetParent(_baseParent);
        _volumesView.HidePerformed();
        _volumeView.ChangeActive(false);
        _cageContainerItem.UpdatePositionByGroupDelayed();

        if (gameObject.activeSelf)
        {
            _cageContainerItem.UpdatePositionByGroupDelayed();
            StartCoroutine(DelayedDeactivate());
        }
        else
        {
            _cageContainerItem.UpdatePositionByGroup();
            gameObject.SetActive(false);
        }
    }

    private void OnTrigger()
    {
        var ability = _volumesView.CurrentData;
        _cage.AddVolume(ability);
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
    }

    private void OnPassiveActivate(IBallVolumeViewData data)
    {
        _volumesView.HidePerformed();
        _volumeView.ChangeActive(false, data);
    }

    private IEnumerator DelayedDeactivate()
    {
        yield return new WaitForSeconds(ShowDuration);
        gameObject.SetActive(false);
    }
}
