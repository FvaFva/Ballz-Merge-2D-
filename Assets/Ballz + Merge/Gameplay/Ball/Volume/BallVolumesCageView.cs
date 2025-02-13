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

    public IEnumerable<BallVolumesBagCell> ActiveVolumes => _cage.Select(x=>x.Current);

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
                cageElement.Activate().Apply(ballVolume);
                return;
            }
        }

        _elements.Add(Instantiate(_prefab, transform).Apply(ballVolume).ConnectContainer(_container));
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

    public int CheckNext(BallVolumesTypes type)
    {
        if(_cage.Count == 0)
            return 0;

        if(_cage.Peek().Current.IsEqual(type))
        {
            var cell = _cage.Dequeue();
            cell.Hide();
            return cell.Current.Value;
        }

        return 0;
    }

    public void Init()
    {
        _elements = new List<BallVolumeCageElement>();
        _cage = new Queue<BallVolumeCageElement>();

        for (int i = 0; i < _countPreload; i++)
            _elements.Add(Instantiate(_prefab, transform).Clear().ConnectContainer(_container));
    }
}