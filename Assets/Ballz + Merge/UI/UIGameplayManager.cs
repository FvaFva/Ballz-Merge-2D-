using R3;
using Zenject;

public class UIGameplayManager : UIRootManager
{
    private readonly Subject<Unit> _exitSceneRequest;

    public UIGameplayManager(DiContainer container) : base(container)
    {
        _exitSceneRequest = container.Resolve<Subject<Unit>>();
    }

    public ScreenGameplayViewModel OpenScreenGameplay()
    {
        ScreenGameplayViewModel viewModel = new(this, _exitSceneRequest);
        UIGameplayViewModel rootUI = Container.Resolve<UIGameplayViewModel>();

        rootUI.OpenScreen(viewModel);

        return viewModel;
    }

    public PopupScoreViewModel OpenPopupScore()
    {
        PopupScoreViewModel score = new();
        UIGameplayViewModel rootUI = Container.Resolve<UIGameplayViewModel>();

        rootUI.OpenPopup(score);

        return score;
    }

    public PopupValuesViewModel OpenPopupValues()
    {
        PopupValuesViewModel values = new();
        UIGameplayViewModel rootUI = Container.Resolve<UIGameplayViewModel>();

        rootUI.OpenPopup(values);

        return values;
    }
}
