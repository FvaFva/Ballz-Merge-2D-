using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GooglePalyGamesManager : MonoBehaviour
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private TMP_Text _debugView;

    private void OnEnable()
    {
        _signInButton.AddListener(Authenticate);
    }

    public void Authenticate()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(AuthenticateCallback);
    }

    private void AuthenticateCallback(SignInStatus status)
    {
        if(status == SignInStatus.Success)
        {
            Debug.Log("So good");
            _debugView.text = $"Name: {PlayGamesPlatform.Instance.GetUserDisplayName()}\n ID: {PlayGamesPlatform.Instance.GetUserId()}";
        }
        else
        {
            Debug.Log("Failed to retrieve GPS auth code");
        }
    }
}
