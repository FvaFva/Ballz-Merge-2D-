using BallzMerge.Gameplay.BlockSpace;
using System;
using System.Collections;
using UnityEngine;

public abstract class AdditionalEffectBase : EffectBase
{
    [SerializeField] protected BlockAdditionalEffectEvents CurrentEvent;

    public new event Action<AdditionalEffectBase> Played;

    private void Awake()
    {
        Played += OnPlayed;
    }

    public BlockAdditionalEffectEvents ResponsibleEvent => CurrentEvent;

    public void OnPlayed(EffectBase effect)
    {
        Played?.Invoke(this);
    }
}
