using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallVolumeCarrier : CyclicBehavior , IDropHandler, IInitializable
{
    private const float AnimationTime = 0.15f;
    private const string ToBagHeader = "Drop here for bag";
    private const string ToCageHeader = "Click here for cage";

    [Header("Logic")]
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

    private Tween _lockAnimation;
    private float _lockBaseFade;

    private void Awake()
    {
        _lockBaseFade = _lock.color.a;
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
        var ability = _container.Swap(default);
        _volumes.Bag.ApplyVolume(ability.Volume, ability.Rarity);
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
    }

    private void ChangeActive(bool state)
    {
        StopAnimations();
        _pumper.enabled = state;
        _button.enabled = state;
        _trigger.enabled = state;
        float targetFade = state ? 0 : _lockBaseFade;
        float targetScale = state ? 2 : 1;
        _headerParent.gameObject.SetActive(state);

        _lockAnimation = DOTween.Sequence()
            .Join(_lock.DOFade(targetFade, AnimationTime))
            .Join(_lock.transform.DOScale(targetScale, AnimationTime).SetEase(Ease.InOutBounce));
    }

    private void StopAnimations()
    {
        if(_lockAnimation != null && _lockAnimation.IsActive())
            _lockAnimation.Kill();
    }

    public void Init() { }
}
