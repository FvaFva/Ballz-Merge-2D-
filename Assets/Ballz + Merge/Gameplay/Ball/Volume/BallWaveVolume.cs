using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallWaveVolume : CyclicBehavior, IWaveUpdater, IInitializable, ILevelFinisher, IDisposable
{
    [SerializeField] private DropSelector _dropSelector;
    [SerializeField] private BallVolumesCageView _cage;

    private Func<IEnumerable<BallVolumesBagCell>> _allVolumesGenerator = () => (Enumerable.Empty<BallVolumesBagCell>());
    public BallVolumesBag Bag {  get; private set; }
    public BallVolumesCageView Cage => _cage;

    public event Action Changed;

    private void OnEnable()
    {
        _dropSelector.Opened += OnDropOpened;
    }

    private void OnDisable()
    {
        _dropSelector.Opened -= OnDropOpened;
    }

    public void Dispose()
    {
        Bag.Added -= OnBagChanged;
        Bag.Removed -= OnBagChanged;
        Bag.Loaded -= OnVolumeBagLoaded;
    }

    public void Init()
    {
        Bag = new BallVolumesBag(_dropSelector);
        Bag.Added += OnBagChanged;
        Bag.Removed += OnBagChanged;
        Bag.Loaded += OnVolumeBagLoaded;
        _cage.Init();
        _allVolumesGenerator = () => Bag.Passive.Concat(_cage.ActiveVolumes).Concat(Bag.Hit);
    }

    public void UpdateWave()
    {
        _cage.RebuildCage();
        Changed?.Invoke();
    }

    public IEnumerable<BallVolumesBagCell> GetAllVolumes() => _allVolumesGenerator();

    public int GetPassiveValue(BallVolumesTypes type) => Bag.Passive.Where(cell => cell.IsEqual(type)).Sum(cell => cell.Value);

    public void FinishLevel()
    {
        _cage.Clear();
        Bag.Clear();
        Changed?.Invoke();
    }

    private void OnBagChanged(BallVolumesBagCell volumeBagCell)
    {
        Changed?.Invoke();
    }

    private void OnVolumeBagLoaded(BallVolumesBagCell volumeBagCell)
    {
        if (volumeBagCell.ID == 0)
            return;

        _cage.AddSavedVolume(volumeBagCell);
        Bag.DropVolume(volumeBagCell);
    }

    private void OnDropOpened()
    {
        _cage.HideAllHightLights();
    }
}