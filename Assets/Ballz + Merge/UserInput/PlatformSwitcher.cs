using UnityEngine;

public class PlatformSwitcher : MonoBehaviour
{
    [SerializeField] private AndroidControlView _androidView;
    [SerializeField] private BallStrikeVectorReader _ballStrikeVectorReader;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            _ballStrikeVectorReader.ChangeToAndroid();
        else
            _androidView.Disable();
    }
}