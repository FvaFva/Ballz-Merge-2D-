using System.Collections.Generic;
using UnityEngine;

public class OrientationDependentPuck : CyclicBehavior, IDependentScreenOrientation
{
    [SerializeField] private List<AdaptiveLayoutGroup> _puck;

    public void UpdateScreenOrientation(ScreenOrientation orientation)
    {
        foreach (var element in _puck)
            element.UpdateScreenOrientation(orientation);
    }
}
