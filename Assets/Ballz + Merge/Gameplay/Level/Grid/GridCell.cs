using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _collider;
        [SerializeField] private MeshRenderer _view;

        private BoxCollider2D _virtualCollider;
        private bool _inited;

        public Vector2Int GridPosition { get; private set; }
        public bool IsActive { get; private set; }

        private void Start()
        {
            _view.material.SetFloat("_randomizerScale", Random.Range(40, 70));
            _view.material.SetFloat("_randomizerTime", Random.Range(0.05f, 0.3f));
        }

        public void Activate(Vector2Int gridPosition, float cellSize, BoxCollider2D virtualCollider)
        {
            if (_inited)
                return;

            _inited = true;
            gameObject.SetActive(true);
            GridPosition = gridPosition;
            transform.localPosition = (Vector2)gridPosition * cellSize;
            _collider.size = new Vector2(cellSize, cellSize);
            name = $"[{GridPosition.x}] - [{GridPosition.y}]";
            ConnectVirtualCollider(virtualCollider);
            ChangeActivity(false);
        }

        public void Deactivate()
        {
            if (_inited == false)
                return;

            _inited = false;
            gameObject.SetActive(false);
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
            if (_virtualCollider != null && transform.position == collider.transform.position)
                return;

            _virtualCollider = collider;
            _virtualCollider.enabled = IsActive;
            _virtualCollider.transform.position = transform.position;
            _virtualCollider.transform.localScale = transform.localScale;
            _virtualCollider.size = _collider.size;
            _virtualCollider.name = $"[{GridPosition.x}] - [{GridPosition.y}]";
        }
    }
}