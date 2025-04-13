using System.Collections.Generic;
using UnityEngine;

public class BallFieldSpreader : BallComponent
{
    private const string PropertyName = "_Position";

    [SerializeField] private List<ParticleSystemRenderer> _fields;

    private List<Material> _materials;
    private Transform _transform;
    private Vector3 _outfield = Vector3.one * 900;

    private void Awake()
    {
        _materials = new List<Material>();

        foreach (var field in _fields)
            _materials.Add(field.material);

        _transform = transform;
    }

    private void OnDisable()
    {
        UpdateProperty(_outfield);
    }

    private void Update()
    {
        UpdateProperty(_transform.position);
    }

    private void UpdateProperty(Vector3 value)
    {
        foreach (var fieldMaterial in _materials)
            fieldMaterial.SetVector(PropertyName, value);
    }
}
