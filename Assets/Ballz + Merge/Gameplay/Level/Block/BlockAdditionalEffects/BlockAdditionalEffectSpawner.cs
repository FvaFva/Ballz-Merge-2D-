using System.Collections.Generic;
using UnityEngine;

public class BlockAdditionalEffectSpawner : CyclicBehaviour, IInitializable
{
    [SerializeField] private BlocksBus _blockBus;
    [SerializeField] private BlockAdditionalEffectSettings _settings;

    private void OnEnable()
    {
        _blockBus.WaveSpawned += HandleWave;
    }

    private void OnDisable()
    {
        _blockBus.WaveSpawned -= HandleWave;
    }

    public void Init()
    {
        
    }

    public void HandleWave(IEnumerable<Block> wave)
    {
        BlockAdditionalEffect prefab = _settings.GetPrefab();
        BlockAdditionalEffect effect = Instantiate(prefab);
        effect.Init(_blockBus);
    }
}
