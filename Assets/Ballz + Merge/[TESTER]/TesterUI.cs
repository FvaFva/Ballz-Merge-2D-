using UnityEngine;
using UnityEngine.UI;

public class TesterUI : MonoBehaviour
{
    [SerializeField] private Button _resolve;
    [SerializeField] private GameCycler _loader;

    private void Awake()
    {
#if UNITY_EDITOR
        _resolve.AddListener(InstantiateScene);
#else
        Destroy(gameObject)
#endif
    }

    private void InstantiateScene()
    {
        foreach (IInitializable initializable in _loader.Initializables)
            initializable.Init();
        

        _loader.RestartLevel();
    }
}
