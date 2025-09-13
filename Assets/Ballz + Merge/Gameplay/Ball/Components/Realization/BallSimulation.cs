using UnityEngine;

public class BallSimulation : BallComponent
{
    private const int BaseCollisions = 1;

    [SerializeField] private float _simulationForce;
    [SerializeField] private BallWaveVolume _waveVolume;

    private Transform _transform;

    public int CollisionsLeft { get; private set; }
    public Vector2 Position => _transform.position;

    private void Awake()
    {
        _transform = transform;
    }

    public void Restart(Vector2 position, Vector2 direction)
    {
        CollisionsLeft = BaseCollisions + _waveVolume.GetPassiveValue<BallVolumeStarlight>();
        _transform.position = position;
        MyBody.linearVelocity = Vector2.zero;
        MyBody.AddForce(direction * _simulationForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionsLeft--;
    }
}