using UnityEngine;
using R3;
using System.Collections.Generic;

public class WindowsContainer : MonoBehaviour
{
    [SerializeField] private Transform _screensContainer;
    [SerializeField] private Transform _popupsContainer;

    private readonly Dictionary<WindowViewModel, IWindowBinder> _openedPopupsBinders = new();
    private IWindowBinder _openedScreenBinder;

    public void OpenPopup(WindowViewModel viewModel)
    {
        string prefabPath = GetPrefabPath(viewModel);
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        GameObject createdPopup = Instantiate(prefab, _popupsContainer);
        IWindowBinder binder = createdPopup.GetComponent<IWindowBinder>();

        binder.Bind(viewModel);
        _openedPopupsBinders.Add(viewModel, binder);
    }

    public void ClosePopup(WindowViewModel popupViewModel)
    {
        IWindowBinder binder = _openedPopupsBinders[popupViewModel];

        binder?.Close();
        _openedPopupsBinders.Remove(popupViewModel);
    }

    public void OpenScreen(WindowViewModel screenViewModel)
    {
        if (screenViewModel == null)
            return;

        _openedScreenBinder?.Close();
        string prefabPath = GetPrefabPath(screenViewModel);
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        GameObject createdScreen = Instantiate(prefab, _screensContainer);
        IWindowBinder binder = createdScreen.GetComponent<IWindowBinder>();

        binder?.Bind(screenViewModel);
        _openedScreenBinder = binder;
    }

    private static string GetPrefabPath(WindowViewModel viewModel)
    {
        return $"UI/{viewModel.Id}";
    }    
}
