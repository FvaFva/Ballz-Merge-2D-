using UnityEngine;
using static UnityEngine.ParticleSystem;

public class UIParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;

    private RectTransform _transform;
    private ShapeModule _shapeModule;
    private ExternalForcesModule _externalForcesModule;

    public void Init()
    {
        _transform = GetComponent<RectTransform>();
        _shapeModule = _particle.shape;
        _externalForcesModule = _particle.externalForces;
        _particle.Stop();
    }

    public void Play(ParticleSystemForceField field)
    {
        _externalForcesModule.AddInfluence(field);
        Play();
    }

    public void Play()
    {
        Vector2 scale = _transform.rect.size * _transform.lossyScale;
        _shapeModule.scale = new Vector3(scale.x, scale.y, 0f);
        _particle.Play();
    }

    public void Stop()
    {
        _externalForcesModule.RemoveAllInfluences();
        _particle.Stop();
    }
}
