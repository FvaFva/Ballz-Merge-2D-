using System.Collections.Generic;
using UnityEngine;

public class BallVolumesCageView : MonoBehaviour, IInitializable
{
    [SerializeField] private BallVolumeCageElement _prefab;

    private List<BallVolumeCageElement> _elements;
    private Queue<BallVolumesBagCell> _cage;

    public IEnumerable<BallVolumesBagCell> ActiveVolumes => _cage;

    public void Clear()
    {
        _cage.Clear();

        foreach (BallVolumeCageElement cageElement in _elements)
            cageElement.Hide(true);
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

        _elements.Add(Instantiate(_prefab, transform).Apply(ballVolume));
    }

    public void RebuildCage()
    {
        _cage.Clear();

        foreach (BallVolumeCageElement cageElement in _elements)
            _cage.Enqueue(cageElement.Current);
    }

    public int CheckNext(BallVolumesTypes type)
    {
        if(_cage.Count == 0)
            return 0;

        if(_cage.Peek().IsEqual(type))
            return _cage.Dequeue().Value;

        return 0;
    }

    public void Init()
    {
        _elements = new List<BallVolumeCageElement>();
        _cage = new Queue<BallVolumesBagCell>();
    }
}