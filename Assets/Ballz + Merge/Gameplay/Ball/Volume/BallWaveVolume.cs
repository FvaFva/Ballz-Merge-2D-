using BallzMerge.Data;
using BallzMerge.Gameplay.Level;
using System;
using System.Linq;
using UnityEngine;

public class BallWaveVolume : CyclicBehavior, IWaveUpdater, IInitializable, ILevelFinisher, IDisposable, IHistorical
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

    public GameHistoryData Write(GameHistoryData data)
    {
        data.Volumes = Bag.All.Select(ballVolume => new { ballVolume.Name, ballVolume.Value })
            .Concat(Cage.Elements.Where(cage => cage.Current != null).Select(cage => new { cage.Current.Name, cage.Current.Value }))
            .GroupBy(name => name.Name)
            .ToDictionary(name => name.Key, values => values.Select(value => value.Value).ToList());

        return data;
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

    public int GetPassiveValue(Type type)=> Bag.Passive.Where(c => c.IsEqual(type)).Count();
    
    public int GetPassiveValue<T>() where T : BallVolumePassive => Bag.Passive.Where(c => c.IsEqual<T>()).Count(); 

    public void FinishLevel()
    {
        _cage.Clear();
        Bag.Clear();
        Changed?.Invoke();
    }

    private void OnBagChanged(IBallVolumesBagCell<BallVolume> _)
    {
        Changed?.Invoke();
    }

    private void OnVolumeBagLoaded(IBallVolumesBagCell<BallVolume> volumeBagCell)
    {
        if (volumeBagCell is BallVolumesBagCell<BallVolumeOnHit> onHit && onHit.ID != 0)
        {
            _cage.AddSavedVolume(onHit);
            Bag.DropVolume(onHit);
        }
    }

    private void OnDropOpened()
    {
        _cage.HideAllHightLights();
    }
}