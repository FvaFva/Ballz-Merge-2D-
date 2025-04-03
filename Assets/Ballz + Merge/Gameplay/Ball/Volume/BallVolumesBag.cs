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
    }

    public IList<BallVolumesBagCell> Hit => _hit;
    public IList<BallVolumesBagCell> Passive => _passive;
    public IEnumerable<BallVolumesBagCell> All => _all;

    public event Action Changed;
    public event Action<BallVolumesBagCell> Added;

    public void Dispose()
    {
        _dropSelector.DropSelected -= ApplyVolume;
    }

    public void Clear()
    {
        _hit.Clear();
        _passive.Clear();
        _all.Clear();
        Changed?.Invoke();
    }

    private void ApplyVolume(BallVolume volume, DropRarity rarity)
    {
        BallVolumesBagCell newCell = new BallVolumesBagCell(volume, rarity);
        _all.Add(newCell);

        switch (volume.Species)
        {
            case BallVolumesSpecies.Passive:
                _passive.Add(newCell);
                break;
            case BallVolumesSpecies.Hit:
                _hit.Add(newCell);
                break;
        }

        Changed?.Invoke();
        Added?.Invoke(newCell);
    }
}