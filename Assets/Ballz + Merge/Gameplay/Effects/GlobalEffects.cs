using UnityEngine;
using Zenject;

public class GlobalEffects : MonoBehaviour
{
    [SerializeField] private ScreenInteraction _interaction;

    [Inject] private DiContainer _container;

    public void Init()
    {
        _container.Inject(_interaction);
    }
}
