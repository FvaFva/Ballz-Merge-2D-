using UnityEngine;
using UnityEngine.EventSystems;

public class BallVolumeCageElement : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameDataVolumeMicView _view;

    private BallVolumeCageContainer _container;
    private RectTransform _transform;

    public BallVolumesBagCell Current {  get; private set; }
    public bool IsFree { get; private set; }

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
    }

    public BallVolumeCageElement ConnectContainer(BallVolumeCageContainer container)
    {
        _container = container;
        return this;
    }

    public BallVolumeCageElement Apply(BallVolumesBagCell volume)
    {
        if (Current.IsEqual(volume))
            return this;

        if(volume.IsEmpty())
        {
            Clear();
            return this;
        }

        Current = volume;
        ShowCurrent();
        return this;
    }

    public BallVolumeCageElement Clear()
    {
        Current = default;
        IsFree = true;
        Hide();
        return this;
    }

    public void Hide()
    {
        _view.Hide();
    }

    public void Show()
    {
        ShowCurrent();
    }   
    
    public BallVolumeCageElement Activate()
    {
        if (IsFree == false)
            ShowCurrent();
        else
            IsFree = false;
        
        gameObject.SetActive(true);
        return this;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _container.Put(this, _transform.position);
        _view.Hide();
    }

    public void OnDrop(PointerEventData eventData)
    {
        _container.Swap(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _container.ApplyDelta(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _container.Disable();

        if(!Current.IsEmpty())
            ShowCurrent();
    }

    private void ShowCurrent()
    {
        _view.Show(Current.Volume, Current.Value);
    }
}