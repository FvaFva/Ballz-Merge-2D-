using R3;

public class ScreenGameplayViewModel : WindowViewModel
{
    private readonly UIGameplayManager _uiManager;
    private readonly Subject<Unit> _exitSceneRequest;

    public override string Id => "ScreenGameplay";

    public ScreenGameplayViewModel(UIGameplayManager uiManager, Subject<Unit> exitSceneRequest)
    {
        _uiManager = uiManager;
        _exitSceneRequest = exitSceneRequest;
    }

    public void RequestOpenScore()
    {
        _uiManager.OpenPopupScore();
    }

    public void RequestOpenValues()
    {
        _uiManager.OpenPopupValues();
    }

    public void RequestToMainMenu()
    {
        _exitSceneRequest.OnNext(Unit.Default);
    }
}
