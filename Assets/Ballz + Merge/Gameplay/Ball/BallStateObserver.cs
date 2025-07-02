using BallzMerge.Gameplay.BallSpace;
using UnityEngine;

public class BallStateObserver : MonoBehaviour
{
    [SerializeField] private Ball _ball;

    void OnEnable()
    {
        _ball.LeftAIM += () => Debug.Log("_ball.LeftAIM");
        _ball.LeftAwait += () => Debug.Log("_ball.LeftAwait");
        _ball.LeftGame += () => Debug.Log("_ball.LeftGame");
        _ball.EnterAIM += () => Debug.Log("_ball.EnterAIM");
        _ball.EnterAwait += () => Debug.Log("_ball.EnterAwait");
        _ball.EnterGame += () => Debug.Log("_ball.EnterGame");
    }
}