using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;

public class BallVolumesBag : IDisposable
{
    private DropSelector _dropSelector;
    private List<BallVolumesBagCell<BallVolumeOnHit>> _hit;
    private List<BallVolumesBagCell<BallVolumePassive>> _passive;
    private List<IBallVolumesBagCell<BallVolume>> _all;

    public BallVolumesBag(DropSelector dropSelector)
    {
        _hit = new List<BallVolumesBagCell<BallVolumeOnHit>>();
        _passive = new List<BallVolumesBagCell<BallVolumePassive>>();
        _all = new List<IBallVolumesBagCell<BallVolume>>();

        _dropSelector = dropSelector;
        _dropSelector.DropSelected += ApplyVolume;
        _dropSelector.DropLoaded += LoadVolume;
    }

    public IList<BallVolumesBagCell<BallVolumeOnHit>> Hit => _hit;
    public IList<BallVolumesBagCell<BallVolumePassive>> Passive => _passive;
    public IEnumerable<IBallVolumesBagCell<BallVolume>> All => _all;

    public event Action<IBallVolumesBagCell<BallVolume>> Added;
    public event Action<IBallVolumesBagCell<BallVolume>> Removed;
    public event Action<IBallVolumesBagCell<BallVolume>> Loaded;

    public void Dispose()
    {
        _dropSelector.DropSelected -= ApplyVolume;
        _dropSelector.DropLoaded -= LoadVolume;
    }

    public void Clear()
    {
        _hit.Clear();
        _passive.Clear();
        _all.Clear();
    }

    public void DropVolume(IBallVolumesBagCell<BallVolume> bagCell)
    {
        if(bagCell is BallVolumesBagCell<BallVolumeOnHit> hit)
            _hit.Remove(hit);
        else if(bagCell is BallVolumesBagCell<BallVolumePassive> passive)
            _passive.Remove(passive);
        
        _all.Remove(bagCell);
        Removed?.Invoke(bagCell);
    }

    public void ApplyVolume(IBallVolumesBagCell<BallVolume> bagCell)
    {
        _all.Add(bagCell);

        if(bagCell is BallVolumesBagCell<BallVolumeOnHit> hit)
            _hit.Add(hit);
        else if(bagCell is BallVolumesBagCell<BallVolumePassive> passive)
            _passive.Add(passive);

        Added?.Invoke(bagCell);
    }

    private void LoadVolume(IBallVolumesBagCell<BallVolume> bagCell)
    {
        ApplyVolume(bagCell);
        Loaded?.Invoke(bagCell);
    }
}