using BallzMerge.Root;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BallVolumesCageView : MonoBehaviour, IInitializable
{
    private const float SlowMoTime = 1.7f;

    [SerializeField] private Image _block;
    [SerializeField] private BallVolumeCageElement _prefab;
    [SerializeField] private BallVolumeCageContainer _container;
    [SerializeField] private int _countPreload;
    [SerializeField] private RectTransform _box;

    private List<BallVolumeCageElement> _elements = new List<BallVolumeCageElement>();
    private Queue<BallVolumeCageElement> _cage;
    private bool _isInited;

    [Inject] private IGameTimeOwner _timeScaler;

    public IEnumerable<BallVolumesBagCell> ActiveVolumes => _elements.Where(x=>x.IsFree == false).Select(x => x.Current);

    private void OnEnable()
    {
        foreach (var element in _elements)
            element.RequiredSlowMo += OnRequiredSlowMo;
    }

    private void OnDisable()
    {
        foreach (var element in _elements)
            element.RequiredSlowMo -= OnRequiredSlowMo;
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
            if(cageElement.IsFree)
            {
                cageElement.Apply(ballVolume);
                return;
            }
        }

        _elements.Add(GenerateElement());
    }

    public void HideAllHightLights()
    {
        foreach (var element in _elements)
            element.ChangeHighlight(false);
    }

    public void RebuildCage()
    {
        _cage.Clear();

        foreach (BallVolumeCageElement cageElement in _elements.Where(element => !element.IsFree))
        {
            cageElement.Show();
            cageElement.ChangeHighlight(false);
            _cage.Enqueue(cageElement);
        }

        HighlightNext();
    }

    public BallVolumesBagCell CheckNext()
    {
        if(_cage.Count == 0)
            return default;

        var last = _cage.Dequeue();
        HighlightNext();
        last.ChangeHighlight(false);
        return last.Current;
    }

    public void Init()
    {
        if(_isInited) 
            return;

        _isInited = true;
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
        var newElement = Instantiate(_prefab, _box).Init(_container).Clear();
        newElement.RequiredSlowMo += OnRequiredSlowMo;
        return newElement;
    }

    private void OnRequiredSlowMo() => _timeScaler.PlaySlowMo(SlowMoTime);
}