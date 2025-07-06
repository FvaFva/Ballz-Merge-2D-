using BallzMerge.Gameplay.BlockSpace;
using BallzMerge.Gameplay.Level;
using Zenject;

public class BallVolumeHitInspector
{
    [Inject] private readonly BlocksInGame _blocks;
    [Inject] private readonly BallWaveVolume _ballWaveVolume;
    [Inject] private readonly GridSettings _grid;
    [Inject] private readonly BallVolumesMap _map;

    private readonly DiContainer _container;
    
    [Inject]
    public BallVolumeHitInspector(DiContainer diContainer)
    {
        _container = diContainer;
    }

    public void Init()
    {
        foreach (var volume in _map.GetAllByType<BallVolumeOnHit>())
            volume.Init(_blocks, _ballWaveVolume, _grid, _container);
    }

    public void Explore(BallVolumeHitData data)
    {
        var nextVolume = _ballWaveVolume.Cage.CheckNext();

        if (nextVolume != null)
            nextVolume.Volume.Explore(data, nextVolume.Rarity, nextVolume.ViewCallback);
    }
}
