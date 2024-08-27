using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _collider;

        private BoxCollider2D _virtualCollider;
        private bool _inited;

        public Vector2Int GridPosition { get; private set; }
        public bool IsActive { get; private set; }

        public void Init(Vector2Int gridPosition, float cellSize, BoxCollider2D virtualCollider)
        {
            if (_inited)
                return;

            _inited = true;
            GridPosition = gridPosition;
            transform.localPosition = (Vector2)gridPosition * cellSize;
            _collider.size = new Vector2(cellSize, cellSize);
            ConnectVirtualCollider(virtualCollider);
            ChangeActivity(false);
        }

        public void ChangeActivity(bool newState)
        {
            IsActive = newState;
            _collider.enabled = newState;
            _virtualCollider.enabled = newState;
        }

        private void ConnectVirtualCollider(BoxCollider2D collider)
        {
            if (_virtualCollider != null)
                return;

            _virtualCollider = collider;
            _virtualCollider.enabled = IsActive;
            _virtualCollider.transform.position = transform.position;
            _virtualCollider.transform.localScale = transform.localScale;
            _virtualCollider.size = _collider.size;
        }
    }
}