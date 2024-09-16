using BallzMerge.Gameplay.Level;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BallzMerge.Gameplay.BallSpace
{
    public class VirtualWorldFactory : CyclicBehavior, IInitializable
    {
        [SerializeField] private PlayZoneBoards _boards;
        [SerializeField] private GridVirtualCell _prefab;
        [SerializeField] private Transform _boxParentForeMoveToVirtual;

        private Scene _scene;
        private Queue<BoxCollider2D> _colliders = new Queue<BoxCollider2D>();

        public PhysicsScene2D GetPhysicScene() => _scene.GetPhysicsScene2D();

        public BoxCollider2D[,] CreateBoxes(GridSettings settings, int GridSizeX = 1)
        {
            BoxCollider2D[,] boxes = new BoxCollider2D[settings.GridSize.x, settings.GridSize.y];

            for (int i = GridSizeX - 1; i < settings.GridSize.x; i++)
                for (int j = 0; j < settings.GridSize.y; j++)
                    if (_colliders.Count >= settings.GridSize.x * settings.GridSize.y)
                        boxes[i, j] = _colliders.Dequeue();
                    else
                        boxes[i, j] = GenerateBox();

            SceneManager.MoveGameObjectToScene(_boxParentForeMoveToVirtual.gameObject, _scene);

            return boxes;
        }

        public void Init()
        {
            _boxParentForeMoveToVirtual.parent = null;
            _scene = SceneManager.CreateScene("Physics simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            GameObject boards = Instantiate(_boards.GameObject);
            SceneManager.MoveGameObjectToScene(boards, _scene);
        }

        public BallSimulation CreateBall(Ball original)
        {
            Ball simulatingBall = Instantiate(original).PreLoad();
            simulatingBall.EnterSimulation();
            SceneManager.MoveGameObjectToScene(simulatingBall.gameObject, _scene);
            return simulatingBall.GetBallComponent<BallSimulation>();
        }

        private BoxCollider2D GenerateBox()
        {
            BoxCollider2D collider = Instantiate(_prefab, _boxParentForeMoveToVirtual).Collider;
            _colliders.Enqueue(collider);
            return collider;
        }
    }
}