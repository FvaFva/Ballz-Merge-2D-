using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RectPumper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float Scatter = 0.2f;
    private const float Interval = 0.15f;

    [SerializeField, Range(1, 10)] private float _interval;
    [SerializeField, Range(0, 30)] private float _power;
    [SerializeField, Range(0, 1)] private float _scalePower;
    [SerializeField] private bool _isOne;
    [SerializeField] private bool _isPointerReactor;

    private System.Action _reroller = () => { };
    private RectTransform _me;
    private Coroutine _animator;
    private Queue<Vector3> _rotations;
    private Quaternion _rotationBase;
    private Vector2 _scaleBase;
    private Vector2 _scale;
    private Tween _rotator;
    private Tween _scaler;

    private void Awake()
    {
        _me = GetComponent<RectTransform>();
        _rotations = new Queue<Vector3>();
        _rotations.Enqueue(new Vector3(0, 0, -_power));
        _rotations.Enqueue(new Vector3(0, 0, _power));
        _rotationBase = _me.localRotation;
        _scaleBase = _me.localScale;
        _scale = _scaleBase * (1 + _scalePower);
        _reroller = () => {
            _me.localRotation = _rotationBase;
            _me.localRotation *= Quaternion.Euler(0, 0, Random.Range(-_power, _power));};
    }

    private void OnEnable()
    {
        if (_isOne)
            RerollRotation();
        else
            _animator = StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        _rotator.Delete();
        _scaler.Delete();

        if (_animator != null)
            StopCoroutine(_animator);

        _me.localRotation = _rotationBase;
        _me.localScale = _scaleBase;
    }

    public void RerollRotation() => _reroller();

    private IEnumerator Animation()
    {
        var currentSleepTime = Random.Range(_interval - Scatter, _interval + Scatter);
        yield return new WaitForSeconds(currentSleepTime);

        StartAnimation().OnComplete(() => StartAnimation());

        _animator = StartCoroutine(Animation());
    }

    private Tween StartAnimation()
    {
        var direction = _rotations.Dequeue();
        _rotations.Enqueue(direction);
        _rotator = _me.DOLocalRotate(direction, Interval)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);

        return _rotator;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_isPointerReactor)
            ChangeScale(_scale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(_isPointerReactor)
            ChangeScale(_scaleBase);
    }

    private void ChangeScale(Vector2 scale)
    {
        _scaler.Delete();
        _scaler = _me.DOScale(scale, Interval)
            .SetEase(Ease.OutQuad);
    }
}