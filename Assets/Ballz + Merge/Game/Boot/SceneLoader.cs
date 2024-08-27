using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace BallzMerge.Root
{
    public class SceneLoader
    {
        private const float SceneGeneratePrecent = 0.3f;
        private const float SecondsCheckTime = 0.05f;

        private WaitForSeconds _checkTime;
        private LoadScreen _loadView;

        public SceneLoader(LoadScreen loader)
        {
            _checkTime = new WaitForSeconds(SecondsCheckTime);
            _loadView = loader;
        }

        public IEnumerator LoadScene(string name)
        {
            _loadView.Show();

            foreach(var _ in LoadSceneFromBoot(name))
                yield return _checkTime;

            if(name == ScenesNames.GAMEPLAY)
            {
                foreach (var _ in LoadGameplay())
                    yield return _checkTime;
            }
        }

        private IEnumerable LoadSceneFromBoot(string name)
        {
            SceneManager.LoadSceneAsync(ScenesNames.BOOT);
            _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
            yield return null;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
            _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
            yield return null;

            while (asyncLoad.progress < 0.9f)
            {
                _loadView.MoveProgress(SceneGeneratePrecent, SecondsCheckTime);
                yield return null;
            }

            _loadView.MoveProgress(SceneGeneratePrecent, 1);
            yield return null;
        }

        private IEnumerable LoadGameplay()
        {
            while (ProjectContext.Instance.Container.HasBinding<GameCycler>() == false)
            {
                _loadView.MoveProgress(1, SecondsCheckTime);
                yield return null;
            }

            GameCycler loader = ProjectContext.Instance.Container.Resolve<GameCycler>();

            foreach (IInitializable initializable in loader.Initializables)
            {
                _loadView.MoveProgress(1, SecondsCheckTime);
                initializable.Init();
                yield return null;
            }

            loader.RestartLevel();
            _loadView.MoveProgress(1, 1);
            yield return null;
            _loadView.Hide();
        }
    }
}
