using BallzMerge.Gameplay.Level;
using System;
using System.Linq;
using UnityEngine;

public class BallWaveVolume : CyclicBehavior, IWaveUpdater, IInitializable, ILevelFinisher, IDisposable
{
    [SerializeField] private DropSelector _dropSelector;
    [SerializeField] private BallVolumesCageView _cage;

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
    }

    public void UpdateWave()
    {
        _cage.RebuildCage();
        Changed?.Invoke();
    }

    public int GetPassiveValue(BallVolumesTypes type) => Bag.Passive.Where(cell => cell.IsEqual(type)).Count();

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