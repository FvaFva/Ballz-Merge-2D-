using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _view;

        public Vector2Int GridPosition { get; private set; }

        private void Awake()
        {
            _view.material.SetFloat("_randomizerScale", Random.Range(40, 70));
            _view.material.SetFloat("_randomizerTime", Random.Range(0.05f, 0.3f));
        }

        public void Init(Vector2Int gridPosition, float cellSize)
        {
            GridPosition = gridPosition;
            transform.localPosition = (Vector2)gridPosition * cellSize;
            name = $"[{GridPosition.x}] - [{GridPosition.y}]";
            ChangeActivity(true);
        }

        public void ChangeActivity(bool newState)
        {
            gameObject.SetActive(newState);
        }
    }
}