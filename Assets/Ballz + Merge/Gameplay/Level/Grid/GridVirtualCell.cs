using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class GridVirtualCell : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _collider2D;

        private Transform _transform;

        public BoxCollider2D Collider => _collider2D;


        private void Awake()
        {
            _transform = transform;
        }

        public void ChangeActive(bool newState) => _collider2D.enabled = newState;

        public void Move(Vector3 position)=>_transform.position = position; 
    }
}