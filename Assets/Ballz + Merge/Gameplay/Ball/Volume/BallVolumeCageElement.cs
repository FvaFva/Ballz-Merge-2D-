using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallVolumeCageElement : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameDataVolumeMicView _view;
    [SerializeField] private UIParticle _backlight;
    [SerializeField] private UIParticle _highlight;
    [SerializeField] private UIParticle _positive;
    [SerializeField] private UIParticle _negative;

    private BallVolumeCageContainer _container;
    private RectTransform _transform;

    public BallVolumesBagCell Current {  get; private set; }
    public bool IsFree => !Current.IsInited;

    public event Action RequiredSlowMo;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _view.Init();
    }

    public BallVolumeCageElement Apply(BallVolumesBagCell volume)
    {
        if (Current.Equals(volume))
            return this;

        if(!volume.IsInited)
        {
            Clear();
            return this;
        }

        volume.ViewCallback = ShowEffect;
        Current = volume;
        ShowCurrent();
        return this;
    }

    public BallVolumeCageElement Clear()
    {
        Current = default;
        _view.Hide();
        _backlight.Stop();
        _highlight.Stop();
        return this;
    }

    public void ChangeHighlight(bool isActive)
    {
        if(isActive)
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
    }

    public void Show()
    {
        ShowCurrent();
    }   
    
    public BallVolumeCageElement Init(BallVolumeCageContainer container)
    {
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
        if (Current.IsInited == false)
            return;

        _container.Put(this, _transform.position, eventData.position);
        _view.Clear();
        _backlight.Play(_container.Field);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Apply(_container.Swap(Current));
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Current.IsInited)
            _container.ApplyDelta(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _container.Disable();
        _backlight.Stop();

        if (Current.IsInited)
            ShowCurrent();
    }

    private void ShowCurrent()
    {
        _view.Show(Current);
    }
}