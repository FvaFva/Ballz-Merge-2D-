using Zenject;

public abstract class UIRootManager
{
    protected readonly DiContainer Container;

    protected UIRootManager(DiContainer container)
    {
        Container = container;
    }
}
