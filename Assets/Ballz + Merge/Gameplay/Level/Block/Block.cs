using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using Zenject;

public class Block : MonoBehaviour
{
    private const float AnimationTime = 0.3f;
    private const float FadeTime = 0.6f;
    private const float MoveScaleCoefficient = 0.85f;
    private const float BounceScaleCoefficient = 0.15f;
    private const float ShakeTime = 0.15f;
    private const string FadeProperty = "_fade";

    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private TMP_Text _numberView;
    [SerializeField] private MoveColorMap _colorMap;

    [Inject] private GridSettings _gridSettings;

    private Transform _transform;
    private Tweener _moveTween;
    private Vector3 _baseScale;
    private Material _material;

    public Vector2Int GridPosition {  get; private set; }
    public Vector2 WorldPosition => _transform.position;
    public int Number {  get; private set; }

    public event Action<Block> Deactivated;
    public event Action<Block> CameToNewCell;

    private void Awake()
    {
        _transform = transform;
        _baseScale = transform.localScale;
        _material = _view.material;
    }

    public Block Initialize(Transform parent)
    {
        _transform.parent = parent;
        Deactivate();
        return this;
    }

    public void Activate(int number, Vector2Int gridPosition, Color color)
    {
        Number = number;
        _material.DOFloat(1, FadeProperty, FadeTime);
        _view.enabled = true;
        _view.color = color;
        _numberView.enabled = true;
        _numberView.text = number.ToString();
        _transform.localPosition = (Vector2)gridPosition * _gridSettings.CellSize;
        _transform.localScale = _baseScale;
        GridPosition = gridPosition;
    }

    public void Move(Vector2Int step)
    {
        GridPosition += step;
        StopCurrentMoveTween();
        _moveTween = _transform.DOLocalMove((Vector2)GridPosition * _gridSettings.CellSize, AnimationTime).SetEase(Ease.OutBack).OnComplete(() => CameToNewCell?.Invoke(this));
        Vector3 scale = _baseScale;

        if (step.y != 0)
            scale.x *= MoveScaleCoefficient;
        else
            scale.y *= MoveScaleCoefficient;

        _transform.DOScale(scale, AnimationTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localScale = _baseScale);
    }

    public void Merge(Vector3 worldPositionMergedBlock)
    {
        GridPosition = Vector2Int.zero;
        _numberView.enabled = false;
        StopCurrentMoveTween();
        Vector3 midpoint = Vector3.Lerp(WorldPosition, worldPositionMergedBlock, 0.5f);
        _transform.DOMove(midpoint, AnimationTime).OnComplete(Deactivate);
        _transform.DOShakeScale(AnimationTime, 0.3f, 50, 200);
        _transform.DOScale(_baseScale * 0.5f, AnimationTime);
        _material.DOFloat(0, FadeProperty, FadeTime);
    }

    public void ReduceNumber()
    {
        Number--;
        _numberView.text = Number.ToString();
        _view.color = _colorMap.GetColor(Number);

        if (Number == 0)
            Merge(transform.localPosition);
    }

    public void Shake(Vector2 direction)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_transform.DOLocalMove((Vector2)_transform.localPosition + (direction * BounceScaleCoefficient), ShakeTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localPosition = (Vector2)GridPosition * _gridSettings.CellSize));
        float xScale = _transform.localScale.x * (1 + (direction.y == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
        float yScale = _transform.localScale.y * (1 + (direction.x == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
        sequence.Join(_transform.DOScale(new Vector3(xScale, yScale), ShakeTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localScale = _baseScale));
        sequence.Play();
    }

    public void Deactivate()
    {
        _view.enabled = false;
        _numberView.enabled = false;
        _transform.localPosition = Vector2.zero;
        Deactivated?.Invoke(this);
    }

    private void StopCurrentMoveTween()
    {
        if (_moveTween != null && _moveTween.IsActive())
            _moveTween.Kill();
    }
}
