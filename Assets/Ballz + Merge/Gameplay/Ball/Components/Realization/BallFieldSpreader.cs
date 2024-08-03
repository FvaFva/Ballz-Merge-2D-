using UnityEngine;

public class BallFieldSpreader : BallComponent
{
    private const string PropertyName = "_Position";

    [SerializeField] private ParticleSystemRenderer _field;

    private Material _fieldMaterial;
    private Transform _transform;
    private Vector3 _outfield = Vector3.one * 900;

    private void Awake()
    {
        _fieldMaterial = _field.material;
        _transform = transform;
    }

    private void OnDisable()
    {
        _fieldMaterial.SetVector(PropertyName, _outfield);
    }

    private void Update()
    {
        _fieldMaterial.SetVector(PropertyName, _transform.position);
    }
}
