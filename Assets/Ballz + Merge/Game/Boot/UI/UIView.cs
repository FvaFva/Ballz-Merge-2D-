using BallzMerge.Root;
using System.Collections.Generic;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField] private bool _isUseSettingsQuiteButton;
    [SerializeField] private bool _isUseSettingsMaineMenuButton;
    [SerializeField] private List<UIRootContainerItem> _items;

    private Transform _baseParent;
    private Transform _transform;

    public bool IsUseSettingsQuiteButton => _isUseSettingsQuiteButton;
    public bool IsUseSettingsMaineMenuButton => _isUseSettingsMaineMenuButton;
    public IEnumerable<UIRootContainerItem> Items => _items;

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