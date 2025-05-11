using UnityEngine;

public class BallVolumesComposer : BallComponent
{
    [SerializeField] private BallVolumesCageView _cage;
    [SerializeField] private BallVolumeCarrier _carrier;
    [SerializeField] private BallWaveVolumeView _bag;

    private void OnEnable()
    {
        SetActive(_carrier, true);
        SetActive(_bag, true);
        SetOnlyView(_cage, false);
    }

    private void OnDisable()
    {
        SetActive(_carrier, false);
        SetActive(_bag, false);
        SetOnlyView(_cage, true);
    }

    private void SetActive(Component component, bool state)
    {
        if (component != null)
            component.gameObject.SetActive(state);
    }

    private void SetOnlyView(BallVolumesCageView component, bool state)
    {
        if (component != null)
            component.SetOnlyView(state);
    }
}
