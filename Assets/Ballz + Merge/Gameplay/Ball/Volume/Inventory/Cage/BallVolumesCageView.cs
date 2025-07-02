using BallzMerge.Root;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BallVolumesCageView : MonoBehaviour, IInitializable
{
    private const float SlowMoTime = 0.7f;

    [SerializeField] private Image _block;
    [SerializeField] private BallVolumeCageElement _prefab;
    [SerializeField] private BallVolumeCageContainer _container;
    [SerializeField] private int _countPreload;
    [SerializeField] private RectTransform _box;

    private int _cageID;
    private List<BallVolumeCageElement> _elements = new List<BallVolumeCageElement>();
    private Queue<BallVolumeCageElement> _cage;

    [Inject] private IGameTimeOwner _timeScaler;

    public IEnumerable<BallVolumesBagCell> ActiveVolumes => _elements.Where(x => x.IsFree == false).Select(x => x.Current);

    private void OnEnable()
    {
        foreach (var element in _elements)
            element.RequiredSlowMo += OnRequiredSlowMo;

        _container.Swapped += OnCellSwap;
    }

    private void OnDisable()
    {
        foreach (var element in _elements)
            element.RequiredSlowMo -= OnRequiredSlowMo;

        _container.PerformIfNotNull(container => container.Swapped -= OnCellSwap);
    }

    public void SetOnlyView(bool isOnlyView) => _block.enabled = isOnlyView;

    public void Clear()
    {
        _cage.Clear();

        foreach (BallVolumeCageElement cageElement in _elements)
            cageElement.Clear();
    }

    public void AddVolume(BallVolumesBagCell ballVolume)
    {
        foreach (BallVolumeCageElement cageElement in _elements)
        {
            if (cageElement.IsFree)
            {
                cageElement.Apply(ballVolume);
                break;
            }
        }
        
        RebuildCage();
    }

    public void AddSavedVolume(BallVolumesBagCell savedVolume)
    {
        BallVolumeCageElement cageElement = _elements.Where(element => element.ID == savedVolume.ID).FirstOrDefault();
        cageElement.PerformIfNotNull(cageElement => cageElement.Apply(savedVolume));
    }

    public void HideAllHightLights()
    {
        foreach (var element in _elements)
            element.ChangeHighlight(false);
    }

    public void RebuildCage()
    {
        _cage.Clear();
        var activeElements = _elements.Where(element => !element.IsFree).Select(element => element.Current);

        Queue<BallVolumeCageElement> free = new Queue<BallVolumeCageElement>();

        foreach (BallVolumeCageElement element in _elements)
        {
            if (element.IsFree)
            {
                free.Enqueue(element);
            }
            else
            {
                element.ChangeHighlight(false);

                if (free.TryDequeue(out var lastFree))
                {
                    lastFree.Apply(element.Current);
                    element.Apply(default);
                    _cage.Enqueue(lastFree);
                    free.Enqueue(element);
                }
                else
                {
                    element.Show();
                    _cage.Enqueue(element);
                }
            }
        }

        HighlightNext();
    }

    public BallVolumesBagCell CheckNext()
    {
        if (_cage.Count == 0)
            return default;

        var last = _cage.Dequeue();
        HighlightNext();
        last.ChangeHighlight(false);
        return last.Current;
    }

    public void Init()
    {
        _cageID = 0;
        _cage = new Queue<BallVolumeCageElement>();

        for (int i = 0; i < _countPreload; i++)
            _elements.Add(GenerateElement());
    }

    private void HighlightNext()
    {
        if (_cage.Count == 0)
            return;

        _cage.Peek().ChangeHighlight(true);
    }

    private BallVolumeCageElement GenerateElement()
    {
        var newElement = Instantiate(_prefab, _box).Init(++_cageID, _container).Clear();
        newElement.RequiredSlowMo += OnRequiredSlowMo;
        return newElement;
    }

    private void OnRequiredSlowMo() => _timeScaler.PlaySlowMo(SlowMoTime);

    private void OnCellSwap()
    {
        RebuildCage();
    }
}