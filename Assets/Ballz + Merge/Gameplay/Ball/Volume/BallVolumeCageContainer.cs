using BallzMerge.Root.Audio;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeCageContainer : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _disabledElements;
    [SerializeField] private AudioSourceHandler _audio;

    private BallVolumeCageElement _starter;
    private RectTransform _transform;

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _disabledElements.SetActive(false);
    }

    public void Disable()
    {
        _audio.Play(AudioEffectsTypes.Hit);
        _disabledElements.SetActive(false);
    }

    public void Put(BallVolumeCageElement cell, Vector2 position)
    {
        _icon.enabled = !cell.Current.IsEmpty();
        _audio.Play(AudioEffectsTypes.Hit);

        if (_icon.enabled)
            _icon.sprite = cell.Current.Volume.Icon;

        _disabledElements.SetActive(true);
        _starter = cell;
        _transform.position = position;
    }

    public void Swap(BallVolumeCageElement finisher)
    {
        var startValue = _starter.Current;
        _starter.Apply(finisher.Current);
        finisher.Apply(startValue);
        Disable();
    }

    public void ApplyDelta(Vector2 delta)
    {
        _transform.anchoredPosition += delta;
    }
}
