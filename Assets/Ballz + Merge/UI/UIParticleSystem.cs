using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class UIParticleSystem : MaskableGraphic
{
    [SerializeField] private ParticleSystemRenderer _renderer;
    [SerializeField] private Camera _bakeCamera;
    [SerializeField] private Texture _texture;

    public override Texture mainTexture => _texture ?? base.mainTexture;

    private void Update()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(Mesh mesh)
    {
        mesh.Clear();

        if (_bakeCamera != null && _renderer != null)
            _renderer.BakeMesh(mesh, _bakeCamera);
    }
}
