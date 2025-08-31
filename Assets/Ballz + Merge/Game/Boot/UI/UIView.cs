using BallzMerge.Root;
using System;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField] private bool _isUseSettingsQuiteButton;
    [SerializeField] private bool _isUseSettingsMaineMenuButton;
    [SerializeField] private GameFinishView _finish;
    [SerializeField] private List<UIRootContainerItem> _items;
    [SerializeField] private LayoutAspectBehaviour _rootContainerBehavior;

    private Transform _baseParent;
    private Transform _transform;

    public GameFinishView FinishView => _finish;
    public bool IsUseSettingsQuiteButton => _isUseSettingsQuiteButton;
    public bool IsUseSettingsMaineMenuButton => _isUseSettingsMaineMenuButton;
    public IEnumerable<UIRootContainerItem> Items => _items;
    public LayoutAspectBehaviour RootContainerBehavior => _rootContainerBehavior;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ChangeState(bool state)
    {
        gameObject.SetActive(state);
    }

    public void LeftRoot()
    {
        _transform.SetParent(_baseParent);

        foreach (var item in _items)
            item.UnpackUp();

        gameObject.SetActive(false);
    }
    
    public void Init()
    {
        _baseParent = transform.parent;
        _transform = transform;

        foreach (var item in _items)
            item.Init();
    }
}