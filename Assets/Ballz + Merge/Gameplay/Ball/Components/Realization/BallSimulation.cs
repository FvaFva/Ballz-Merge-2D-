using UnityEngine;

public class BallSimulation : BallComponent
{
    [SerializeField] private float _simulationForce;
    [SerializeField] private int _maxCollisions;

    private Rigidbody2D _rb;
    private Transform _transform;

    public int CollisionsLeft { get; private set; }
    public Vector2 Position => _transform.position;

    private void Awake()
    {
        _rb = GetRigidbody();
        _transform = transform;
    }

    public void Restart(Vector2 position, Vector2 direction)
    {
        CollisionsLeft = _maxCollisions;
        _transform.position = position;
        _rb.velocity = Vector2.zero;
        _rb.AddForce(direction * _simulationForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionsLeft--;
    }
}