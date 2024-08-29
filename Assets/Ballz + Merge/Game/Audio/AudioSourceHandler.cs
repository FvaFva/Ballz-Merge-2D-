using UnityEngine;
using Zenject;

[RequireComponent (typeof(AudioSource))]
public class AudioSourceHandler : MonoBehaviour
{
    private AudioSource _audio;

    [Inject] private AudioSettings _settings;

    private void Awake()
    {
        if(_settings == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _audio = GetComponent<AudioSource>();
        _audio.enabled = _settings.IsActive;
    }

    private void OnEnable()
    {
        _settings.Changed += OnSettingsChanged;
    }

    private void OnDisable()
    {
        if (_settings != null)
            _settings.Changed -= OnSettingsChanged;
    }

    public void Play()
    {
        if (_settings.IsActive)
            _audio.Play();
    }

    private void OnSettingsChanged()
    {
        _audio.enabled = _settings.IsActive;
    }
}