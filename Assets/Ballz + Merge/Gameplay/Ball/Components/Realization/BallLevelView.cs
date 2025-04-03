using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BallLevelView : BallComponent
{
    private const int DisableValue = 0;

    [SerializeField] private List<BallVolumeEffectHandler> _effects;

    [Inject] private BallWaveVolume _volume;

    private void OnEnable()
    {
        //_volume.Changed += OnValueChanged;
    }

    private void OnDisable()
    {
        //_volume.Changed -= OnValueChanged;
    }

    public override void ChangeActivity(bool isActive)
    {
        if(isActive)
        {
            foreach (var effect in _effects)
                effect.HandleVolumeChanges(_volume.GetPassiveValue(effect.Type));
        }
        else
        {
            foreach (var effect in _effects)
                effect.HandleVolumeChanges(DisableValue);
        }
    }

    private void OnValueChanged(BallVolumesTypes type, float value)
    {
        BallVolumeEffectHandler currentEffect = _effects.Where(effect => effect.Type == type).FirstOrDefault();

        if (currentEffect.IsEmpty())
            return;
        else
            currentEffect.HandleVolumeChanges(value);
    }
}
