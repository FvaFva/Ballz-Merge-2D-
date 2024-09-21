using UnityEngine;

public class PlatformSwitcher : MonoBehaviour
{
    [SerializeField] private BallStrikeVectorReader _ballStrikeVectorReader;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
            _ballStrikeVectorReader.ChangeToAndroid();
    }
}