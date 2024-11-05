using Zenject;

public class AchievementSource : CyclicBehavior, IInitializable
{
    [Inject] private DiContainer _container;
    [Inject] private AchievementSettings _settings;

    private AchievementObserverBlocksDestroyer _destroyer;

    public void Init()
    {
        _destroyer = new(_settings, 1);
        _container.Inject(_destroyer);
    }
}
