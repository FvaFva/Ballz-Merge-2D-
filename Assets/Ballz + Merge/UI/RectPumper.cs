using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectPumper : MonoBehaviour
{
    private const float Scatter = 0.2f;
    private const float Interval = 0.15f;

    [SerializeField, Range(1, 10)] private float _interval;
    [SerializeField, Range(0, 30)] private float _power;

    private RectTransform _me;
    private Coroutine _animator;
    private Queue<Vector3> _rotations;
    private Quaternion _rotationBase;
    private Tween _rotator;

    private void Awake()
    {
        _me = GetComponent<RectTransform>();
        _rotations = new Queue<Vector3>();
        _rotations.Enqueue(new Vector3 (0, 0, -_power));
        _rotations.Enqueue(new Vector3 (0, 0, _power));
        _rotationBase = _me.rotation;
    }

    private void OnEnable()
    {
        _animator = StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        if(_rotator != null && _rotator.IsActive())
            _rotator.Kill();

        StopCoroutine(_animator);
        _me.rotation = _rotationBase;
    }

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
        _rotator = _me.DORotate(direction, Interval)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);

        return _rotator;
    }
}