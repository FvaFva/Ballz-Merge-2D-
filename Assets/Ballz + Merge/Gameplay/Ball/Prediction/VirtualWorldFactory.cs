using BallzMerge.Gameplay.Level;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BallzMerge.Gameplay.BallSpace
{
    public class VirtualWorldFactory : CyclicBehavior, IInitializable, ILevelFinisher
    {
        [SerializeField] private PlayZoneBoards _boards;
        [SerializeField] private GridVirtualCell _prefab;
        [SerializeField] private Transform _boxParentForeMoveToVirtual;

        private Scene _scene;
        private Queue<BoxCollider2D> _collidersPool = new Queue<BoxCollider2D>();
        private List<BoxCollider2D> _colliders = new List<BoxCollider2D>();

        public PhysicsScene2D GetPhysicScene() => _scene.GetPhysicsScene2D();

        public BoxCollider2D[,] CreateBoxes(GridSettings settings, int GridSizeX = 1, int GridSizeY = 1)
        {
            BoxCollider2D[,] boxes = new BoxCollider2D[settings.GridSize.x, settings.GridSize.y];

            for (int i = GridSizeX - 1; i < settings.GridSize.x; i++)
                for (int j = GridSizeY - 1; j < settings.GridSize.y; j++)
                    boxes[i, j] = GetCollider();

            SceneManager.MoveGameObjectToScene(_boxParentForeMoveToVirtual.gameObject, _scene);

            return boxes;
        }

        public void Init()
        {
            _boxParentForeMoveToVirtual.parent = null;
            _scene = SceneManager.CreateScene("Physics simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            SceneManager.MoveGameObjectToScene(_boards.GetSimulationClone(), _scene);
        }

        public void FinishLevel()
        {
            foreach (BoxCollider2D collider in _colliders)
            {
                _collidersPool.Enqueue(collider);
                collider.gameObject.SetActive(false);
            }
        }

        public BallSimulation CreateBall(Ball original)
        {
            Ball simulatingBall = Instantiate(original).PreLoad();
            simulatingBall.EnterSimulation();
            SceneManager.MoveGameObjectToScene(simulatingBall.gameObject, _scene);
            return simulatingBall.GetBallComponent<BallSimulation>();
        }

        private BoxCollider2D GetCollider()
        {
            if (_collidersPool.TryDequeue(out BoxCollider2D collider))
                collider.gameObject.SetActive(true);
            else
                collider = GenerateBox();

            return collider;
        }

        private BoxCollider2D GenerateBox()
        {
            BoxCollider2D collider = Instantiate(_prefab, _boxParentForeMoveToVirtual).Collider;
            _colliders.Add(collider);
            return collider;
        }
    }
}