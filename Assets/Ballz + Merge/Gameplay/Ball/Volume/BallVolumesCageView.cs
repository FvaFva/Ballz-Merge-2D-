using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallVolumesCageView : MonoBehaviour, IInitializable
{
    [SerializeField] private BallVolumeCageElement _prefab;
    [SerializeField] private BallVolumeCageContainer _container;
    [SerializeField] private int _countPreload;

    private List<BallVolumeCageElement> _elements;
    private Queue<BallVolumeCageElement> _cage;

    public IEnumerable<BallVolumesBagCell> ActiveVolumes => _elements.Where(x=>x.IsFree == false).Select(x => x.Current);

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

        _elements.Add(Instantiate(_prefab, transform).Init(_container).Apply(ballVolume));
    }

    public void RebuildCage()
    {
        _cage.Clear();

        foreach (BallVolumeCageElement cageElement in _elements.Where(element => !element.IsFree))
        {
            cageElement.Show();
            _cage.Enqueue(cageElement);
        }
    }

    public BallVolumesBagCell CheckNext()
    {
        if(_cage.Count == 0)
            return default;

        return _cage.Dequeue().Current;
    }

    public void Init()
    {
        _elements = new List<BallVolumeCageElement>();
        _cage = new Queue<BallVolumeCageElement>();

        for (int i = 0; i < _countPreload; i++)
            _elements.Add(Instantiate(_prefab, transform).Clear().Init(_container));
    }
}