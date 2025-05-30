using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TesterUI : MonoBehaviour
{
    [SerializeField] private Button _resolve;
    [SerializeField] private GameCycler _loader;

    private void Awake()
    {
#if UNITY_EDITOR
        _resolve.AddListener(InstantiateScene);
#else
        Destroy(gameObject);
#endif
    }

    private void InstantiateScene()
    {
        foreach (IInitializable initializable in _loader.InitializedComponents)
            initializable.Init();
        
        _loader.Init(SceneExitCallBack, false);
    }

    private void SceneExitCallBack(SceneExitData exitData)
    {
        if (exitData.IsGameQuit)
            QuitGame();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
