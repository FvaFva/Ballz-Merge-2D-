using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField] private bool _isUseSettingsQuiteButton;
    [SerializeField] private bool _isUseSettingsMaineMenuButton;

    private Transform _baseParent;
    private Transform _transform;

    public bool IsUseSettingsQuiteButton => _isUseSettingsQuiteButton;
    public bool IsUseSettingsMaineMenuButton => _isUseSettingsMaineMenuButton;

    private void Awake()
    {
        _baseParent = transform.parent;
        _transform = transform;
        gameObject.SetActive(false);
    }

    public void MoveToContainer(RectTransform container)
    {
        _transform.SetParent(container);
        gameObject.SetActive(true);
    }

    public void LeftRoot()
    {
        _transform.SetParent(_baseParent);
        gameObject.SetActive(false);
    }
}