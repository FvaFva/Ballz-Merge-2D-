using BallzMerge.Gameplay.Level;
using UnityEngine;
using Zenject;

public class PlayZoneViewPositioner : MonoBehaviour
{
    private void Awake()
    {
        transform.localPosition = ProjectContext.Instance.Container.Resolve<GridSettings>().ViewPosition;
    }
}
