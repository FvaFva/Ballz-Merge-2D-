using BallzMerge.Gameplay.Level;
using System;
using System.Collections.Generic;

public class BallVolumesBag : IDisposable
{
    private DropSelector _dropSelector;
    private List<BallVolumesBagCell> _hit;
    private List<BallVolumesBagCell> _passive;
    private List<BallVolumesBagCell> _all;

    public BallVolumesBag(DropSelector dropSelector)
    {
        _hit = new List<BallVolumesBagCell>();
        _passive = new List<BallVolumesBagCell>();
        _all = new List<BallVolumesBagCell>();

        _dropSelector = dropSelector;
        _dropSelector.DropSelected += ApplyVolume;
        _dropSelector.DropLoaded += LoadVolume;
    }

    public IList<BallVolumesBagCell> Hit => _hit;
    public IList<BallVolumesBagCell> Passive => _passive;
    public IEnumerable<BallVolumesBagCell> All => _all;

    public event Action<BallVolumesBagCell> Added;
    public event Action<BallVolumesBagCell> Removed;
    public event Action<BallVolumesBagCell> Loaded;

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

    public void DropVolume(BallVolumesBagCell volume)
    {
        _hit.Remove(volume);
        _all.Remove(volume);
        _passive.Remove(volume);
        Removed?.Invoke(volume);
    }

    public void ApplyVolume(BallVolumesBagCell bagCell)
    {
        _all.Add(bagCell);

        switch (bagCell.Volume.Species)
        {
            case BallVolumesSpecies.Passive:
                _passive.Add(bagCell);
                break;
            case BallVolumesSpecies.Hit:
                _hit.Add(bagCell);
                break;
        }

        Added?.Invoke(bagCell);
    }

    private void LoadVolume(BallVolumesBagCell bagCell)
    {
        ApplyVolume(bagCell);
        Loaded?.Invoke(bagCell);
    }
}