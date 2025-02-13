using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallWaveVolume : CyclicBehavior, IWaveUpdater, IInitializable, ILevelFinisher
{
    [SerializeField] private DropSelector _dropSelector;
    [SerializeField] private BallVolumesCageView _cage;

    private Func<IEnumerable<BallVolumesBagCell>> _getActiveVolumesGenerator = () => (Enumerable.Empty<BallVolumesBagCell>());

    public BallVolumesBag Bag {  get; private set; }

    public event Action Changed;

    public void Init()
    {
        Bag = new BallVolumesBag(_dropSelector);
        Bag.Added += OnBagChanged;
        _cage.Init();
        _getActiveVolumesGenerator = () => Bag.Passive.Concat(_cage.ActiveVolumes);
    }

    public void UpdateWave()
    {
        _cage.RebuildCage();
    }

    public IEnumerable<BallVolumesBagCell> GetActiveVolumes() => _getActiveVolumesGenerator();

    public int GetPassiveValue(BallVolumesTypes type)
    {
        return Bag.Passive.Where(cell => cell.IsEqual(type)).Sum(cell => cell.Value);
    }

    public int GetCageValue(BallVolumesTypes type)
    {
        int result = _cage.CheckNext(type);

        if (result != 0)
            Changed?.Invoke();

        return result;
    }

    public void FinishLevel()
    {
        _cage.Clear();
        Bag.Clear();
        Changed?.Invoke();
    }

    private void OnBagChanged(BallVolumesBagCell cell)
    {
        if(cell.IsEqual(BallVolumesSpecies.Hit))
            _cage.AddVolume(cell);

        Changed?.Invoke();
    }
}
