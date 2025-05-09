using UnityEngine;

public class BallVolumesComposer : BallComponent
{
    [SerializeField] private BallVolumesCageView _cage;
    [SerializeField] private BallVolumeCarrier _carrier;
    [SerializeField] private BallWaveVolumeView _bag;

    private void OnEnable()
    {
        _carrier?.gameObject.SetActive(true);
        _bag?.gameObject.SetActive(true);
        _cage?.SetOnlyView(false);
    }

    private void OnDisable()
    {
        _carrier?.gameObject.SetActive(false);
        _bag?.gameObject.SetActive(false);
        _cage?.SetOnlyView(true);
    }
}
