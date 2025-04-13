using System.Collections.Generic;
using UnityEngine;

public class PlayZoneBoard : MonoBehaviour
{
    private const float EffectEdgeScale = 0.5f;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private List<ParticleSystem> _effects;

    private Transform _transform;
    private Vector2 _basePosition;
    private Vector2 _baseScale;
    private Dictionary<ParticleSystem, Vector2>  _baseScales;

    public void MarkAsVirtual()
    {
        if (_renderer != null)
            _renderer.enabled = false;

        _baseScales = new Dictionary<ParticleSystem, Vector2>();

        foreach (var effect in _effects)
            Destroy(effect);

        _effects = new List<ParticleSystem> ();
    }

    public PlayZoneBoard Init()
    {
        _transform = transform;
        _basePosition = _transform.localPosition;
        _baseScale = _transform.localScale;
        _baseScales = new Dictionary<ParticleSystem, Vector2>();

        foreach (var effect in _effects)
            _baseScales.Add(effect, effect.shape.scale);

        return this;
    }

    public void SetBase()
    {
        _transform.localPosition = _basePosition;
        _transform.localScale = _baseScale;

        foreach (var shape in _baseScales)
        {
            var temp = shape.Key.shape;
            temp.scale = shape.Value;
        }
    }

    public void AddProperty(PositionScaleProperty property)
    {
        _transform.localPosition += property.Position;
        _transform.localScale += property.Scale;
        var xScale = property.Scale.magnitude * EffectEdgeScale;

        foreach (var effect in _effects)
        {
            var shape = effect.shape;
            shape.scale = new Vector3(shape.scale.x + xScale, shape.scale.y);
        }
    }

    public void Move(Vector3 step)
    {
        _transform.localPosition += step;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}