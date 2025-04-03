using BallzMerge.Gameplay.Level;
using System;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class BlockPhysicModel : MonoBehaviour
    {
        private BoxCollider2D _box;
        private GridVirtualCell _virtualBox;
        private Transform _transform;

        public Vector3 WorldPosition => _transform.position;
        public event Action<Vector2Int> Hit;

        private void Awake()
        {
            _box = GetComponent<BoxCollider2D>();
            _transform = transform;
        }

        private void OnEnable()
        {
            _virtualBox?.Move(WorldPosition);
        }

        public void Init(GridVirtualCell virtualBox)
        {
            _virtualBox = virtualBox;
            _transform = transform;
            _virtualBox.Move(transform.position);
        }

        public void Activate()
        {
            _box.enabled = true;
            _virtualBox.ChangeActive(true);
            _virtualBox.Move(WorldPosition);
        }

        public void Deactivate()
        {
            _box.enabled = false;
            _virtualBox.ChangeActive(false);
        }

        public void Kick(Vector2Int vector) => Hit?.Invoke(vector);
    }
}
