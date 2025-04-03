using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.ParticleSystem;

public class BallVolumeCageElement : MonoBehaviour, IBeginDragHandler, IDropHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameDataVolumeMicView _view;
    [SerializeField] private ParticleSystem _backlight;

    private BallVolumeCageContainer _container;
    private RectTransform _transform;
    private ShapeModule _shapeModule;
    private ExternalForcesModule _externalForcesModule;

    public BallVolumesBagCell Current {  get; private set; }
    public bool IsFree => !Current.IsInited;

    private void Awake()
    {
        _transform = GetComponent<RectTransform>();
        _shapeModule = _backlight.shape;
        _externalForcesModule = _backlight.externalForces;
        _backlight.Stop();
        
    }

    public BallVolumeCageElement Apply(BallVolumesBagCell volume)
    {
        if (Current.IsEqual(volume))
            return this;

        if(!volume.IsInited)
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
        _view.Hide();
        _backlight.Stop();
        return this;
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
        gameObject.SetActive(true);
        return this;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _container.Put(this, _transform.position, eventData.position);
        _view.Clear();
        HighLightToContainer(true);
    }

    public void OnDrop(PointerEventData eventData)
    {
        _container.Swap(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _container.ApplyDelta(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _container.Disable();
        HighLightToContainer(false);

        if (Current.IsInited)
            ShowCurrent();
    }

    private void ShowCurrent()
    {
        _view.Show(Current.Volume, Current.Value);
    }

    private void HighLightToContainer(bool isActive)
    {
        if(isActive)
        {
            Vector2 scale = _transform.rect.size * _transform.lossyScale;
            _shapeModule.scale = new Vector3(scale.x, scale.y, 0f);
            _externalForcesModule.AddInfluence(_container.Field);
            _backlight.Play();
        }
        else
        {
            _externalForcesModule.RemoveAllInfluences();
            _backlight.Stop();
        }
    }
}