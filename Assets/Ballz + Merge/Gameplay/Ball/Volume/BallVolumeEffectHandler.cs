using System;
using UnityEngine;

[Serializable]
public struct BallVolumeEffectHandler
{
    public ParticleSystem Particles;
    public BallVolumesTypes Type;

    public void HandleVolumeChanges(float value)
    {
        Particles.gameObject.SetActive(value.Equals(0) == false);
    }

    public bool IsEmpty()
    {
        return Particles == null;
    }
}
