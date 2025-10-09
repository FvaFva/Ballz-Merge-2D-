using System.Collections.Generic;
using UnityEngine;

public class OrientationDependentPuck : CyclicBehavior, IDependentScreenOrientation
{
    [SerializeField] private List<AdaptiveLayoutGroupBase> _puck;

    public void UpdateScreenOrientation(bool isVertical)
    {
        foreach (var element in _puck)
            element.UpdateScreenOrientation(isVertical);
    }
}
