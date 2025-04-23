using BallzMerge.Root.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeCageContainer : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _disabledElements;
    [SerializeField] private AudioSourceHandler _audio;
    [SerializeField] private ParticleSystemForceField _field;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private RetainerInsideScreen _description;
    [SerializeField] private TMP_Text _descriptionText;

    private BallVolumeCageElement _starter;
    private RectTransform _transform;
    private RectTransform _transformCanvas;
    private Vector2 _lastLocalPosition;

    public ParticleSystemForceField Field => _field;

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _transformCanvas = (RectTransform)_mainCanvas.transform;
        _disabledElements.SetActive(false);
    }

    public void Disable()
    {
        if (_disabledElements.activeSelf)
        {
            _audio.Play(AudioEffectsTypes.Hit);
            _disabledElements.SetActive(false);
        }
    }

    public void Put(BallVolumeCageElement cell, Vector2 position, Vector2 tupPoint)
    {
        _icon.enabled = cell.Current.IsInited;
        _audio.Play(AudioEffectsTypes.Hit);

        if (_icon.enabled)
            _icon.sprite = cell.Current.Volume.Icon;

        _disabledElements.SetActive(true);
        _descriptionText.text = cell.Current.Volume.GetDescription(cell.Current.Rarity);
        _starter = cell;
        _transform.position = position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _transform.parent as RectTransform,
            tupPoint,
            _mainCanvas.worldCamera,
            out _lastLocalPosition);
    }

    public void Swap(BallVolumeCageElement finisher)
    {
        if (_starter == default)
            return;

        var startValue = _starter.Current;
        _starter.Apply(finisher.Current);
        finisher.Apply(startValue);
        Disable();
    }

    public void ApplyDelta(Vector2 position)
    {
        Vector2 localPoint;
        bool changed = RectTransformUtility.ScreenPointToLocalPointInRectangle(_transformCanvas, position, _mainCanvas.worldCamera, out localPoint);

        if (changed)
        {
            Vector2 delta = localPoint - _lastLocalPosition;
            _transform.anchoredPosition += delta;
            _lastLocalPosition = localPoint;
            _description.UpdatePosition();
        }
    }
}
