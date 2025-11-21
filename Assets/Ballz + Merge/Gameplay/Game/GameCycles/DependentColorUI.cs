using UnityEngine;

public abstract class DependentColorUI : CyclicBehavior, IDependentColorUI
{
    protected GameColors GameColors;

    public abstract void ApplyColors(GameColors gameColors);
}