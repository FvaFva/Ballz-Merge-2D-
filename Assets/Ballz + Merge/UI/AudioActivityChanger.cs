using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AudioActivityChanger : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _active;
    [SerializeField] private Image _inactive;

    [Inject] private AudioSettings _settings;

    private void Awake()
    {
        ChangeActiveImage();
    }

    private void OnEnable()
    {
        _button.AddListener(OnChange);
    }

    private void OnDisable()
    {
        _button.RemoveListener(OnChange);
    }

    private void ChangeActiveImage()
    {
        _active.enabled = _settings.IsActive;
        _inactive.enabled = !_settings.IsActive;
    }

    private void OnChange()
    {
        _settings.Change();
        ChangeActiveImage();
    }
}
