using UnityEngine;
using UnityEngine.SceneManagement;

public class VirtualWorldFactory : CyclicBehavior, IInitializable
{
    [SerializeField] private PalyZoneBoards _boards;
    [SerializeField] private GridVirtualCell _prefab;
    [SerializeField] private Transform _boxParentForeMoveToVirtual;

    private Scene _scene;

    public PhysicsScene2D GetPhysicScene() => _scene.GetPhysicsScene2D();

    public BoxCollider2D[,] CreateBoxes(GridSettings settings)
    {
        BoxCollider2D[,] boxes = new BoxCollider2D[settings.GridSize.x, settings.GridSize.y];

        for (int i = 0; i < settings.GridSize.x; i++)
            for (int j = 0; j < settings.GridSize.y; j++)
                boxes[i, j] = Instantiate(_prefab, _boxParentForeMoveToVirtual).Collider;

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
        Ball simulatingBall = Instantiate(original);
        simulatingBall.EnterSimulation();
        SceneManager.MoveGameObjectToScene(simulatingBall.gameObject, _scene);
        return simulatingBall.GetBallComponent<BallSimulation>();
    }
}