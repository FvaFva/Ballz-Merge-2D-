using UnityEngine;

public class GridVirtualCell : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _collider2D;

    public BoxCollider2D Collider => _collider2D;
}