#if UNITY_EDITOR
using UnityEngine.SceneManagement;

namespace BallzMerge.Root
{
    internal class DebugScenesChecker
    {
        public bool IsItDebug(ref string sceneName, DevelopersScenes devScenes)
        {
            string activeScene = SceneManager.GetActiveScene().name;
            bool isItGameplayOrMainMenu = activeScene == ScenesNames.GAMEPLAY || activeScene == ScenesNames.MAINMENU || devScenes.Scenes.Contains(activeScene);

            if(isItGameplayOrMainMenu)
            {
                sceneName = activeScene;
                return false;
            }

            if (activeScene == ScenesNames.BOOT)
                return false;

            return true;
        }
    }
}
#endif
