using Unity.VisualScripting;
using UnityEngine;

public class BallVolumesComposer : BallComponent
{
    [SerializeField] private BallVolumesCageView _cage;
    [SerializeField] private BallVolumeCarrier _carrier;
    [SerializeField] private BallWaveVolumeView _bag;

    private void OnEnable()
    {
        _cage.PerformIfNotNull(cage => cage.SetOnlyView(false));
    }

    private void OnDisable()
    {
        _cage.PerformIfNotNull(cage => cage.SetOnlyView(true));
    }
}
