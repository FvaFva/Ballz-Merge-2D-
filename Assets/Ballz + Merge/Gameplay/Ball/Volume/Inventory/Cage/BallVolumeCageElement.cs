using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallVolumeCageElement : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameDataVolumeMicView _view;
    [SerializeField] private RectPumper _viewPumper;
    [SerializeField] private UIParticle _backlight;
    [SerializeField] private UIParticle _highlight;
    [SerializeField] private UIParticle _positive;
    [SerializeField] private UIParticle _negative;

    private BallVolumeCageContainer _container;
    private RectTransform _transform;

    public int ID { get; private set; }
    public BallVolumesBagCell<BallVolumeOnHit> Current { get; private set; }
    public bool IsFree => Current == null;

    public event Action RequiredSlowMo;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _view.Init();
    }

    public BallVolumeCageElement Apply(BallVolumesBagCell<BallVolumeOnHit> volume)
    {
        if (Current == volume)
            return this;

        if (volume == null)
        {
            Clear();
            return this;
        }

        volume.SetCallback(ShowEffect);
        volume.SetID(ID);
        Current = volume;
        Show();
        _viewPumper.RerollRotation();
        return this;
    }

    public BallVolumeCageElement Clear()
    {
        Current = default;
        _view.Hide();
        _backlight.Stop();
        _highlight.Stop();
        _viewPumper.RerollRotation();
        return this;
    }

    public void ChangeHighlight(bool isActive)
    {
        if (isActive)
            _highlight.Play();
        else
            _highlight.Stop();
    }

    public void ShowEffect(bool isPositive)
    {
        RequiredSlowMo?.Invoke();

        if (isPositive)
            _positive.Play();
        else
            _negative.Play();
    }

    public void Hide()
    {
        _view.Clear();
        _viewPumper.RerollRotation();
    }

    public void Show()
    {
        _view.Show(Current);
    }

    public BallVolumeCageElement Init(int id, BallVolumeCageContainer container)
    {
        ID = id;
        _container = container;
        _backlight.Init();
        _highlight.Init();
        _positive.Init();
        _negative.Init();
        gameObject.SetActive(true);
        return this;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Current == null)
            return;

        _container.Put(this, _transform.position, eventData.position);
        _view.Clear();
        _backlight.Play(_container.Field);
    }

    public void OnDrop(PointerEventData eventData)
    {
        _container.Swap(Current, ApplyHidden);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Current != null)
            _container.ApplyDelta(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _container.Disable();
        _backlight.Stop();

        if (Current != null)
            Show();
    }

    private void ApplyHidden(BallVolumesBagCell<BallVolumeOnHit> volume)=> Apply(volume);
}