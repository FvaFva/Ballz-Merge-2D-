using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Dropper : CyclicBehaviour, IWaveUpdater, IInitializable, ILevelStarter
{
    [SerializeField] private int _wavesToDrop;
    [SerializeField] private DropSelector _selector;
    [SerializeField] private List<Drop> _drops;
    [SerializeField] private InfoPanel _view;

    private List<Drop> _pool;
    private int _waveCount;

    public void Init()
    {
        _pool = new List<Drop>();

        foreach (Drop drop in _drops)
        {
            for (int i = 0; i < drop.CountInPool; i++)
                _pool.Add(drop);
        }
    }

    public void StartLevel()
    {
        _waveCount = _wavesToDrop;
    }

    public void UpdateWave()
    {
        if(--_waveCount == 0)
        {
            List<Drop> temp = _pool.ToList();
            _selector.Show(temp.TakeRandom(), temp.TakeRandom());
            _waveCount = _wavesToDrop;
        }

        _view.Show(_waveCount);
    }
}