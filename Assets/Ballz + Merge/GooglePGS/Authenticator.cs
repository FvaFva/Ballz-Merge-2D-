#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;

namespace BallzMerge.GooglePGS
{

    public class Authenticator : CyclicBehavior, IInitializable
    {
        public bool IsAuthenticated { get; private set; }
        public event Action Authenticated;

        private void Authenticate()
        {

            PlayGamesPlatform.Activate();
            PlayGamesPlatform.Instance.Authenticate(OnAuthenticate);

}

private void OnAuthenticate(SignInStatus status)
        {
            IsAuthenticated = status != SignInStatus.Success;

            if (IsAuthenticated == false)
                return;

            Authenticated?.Invoke();
        }

        public void Init()
        {
            Authenticate();
        }
    }
}
#endif
