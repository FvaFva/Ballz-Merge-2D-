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

        public PhysicsScene2D GetPhysicScene() => _scene.GetPhysicsScene2D();

        public GridVirtualCell GenerateBoxForBlock()
        {
            return Instantiate(_prefab, _boxParentForeMoveToVirtual);
        }

        public void Init()
        {
            _boxParentForeMoveToVirtual.parent = null;
            _scene = SceneManager.CreateScene("Physics simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            SceneManager.MoveGameObjectToScene(_boards.GetSimulationClone(), _scene);
            SceneManager.MoveGameObjectToScene(_boxParentForeMoveToVirtual.gameObject, _scene);
        }

        public BallSimulation CreateBall(Ball original)
        {
            Ball simulatingBall = Instantiate(original).PreLoad();
            simulatingBall.EnterSimulation();
            SceneManager.MoveGameObjectToScene(simulatingBall.gameObject, _scene);
            return simulatingBall.GetBallComponent<BallSimulation>();
        }
    }
}