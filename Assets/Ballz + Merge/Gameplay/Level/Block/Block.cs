using BallzMerge.Gameplay.Level;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class Block : MonoBehaviour
    {
        private const float FadeTime = 0.6f;
        private const float MoveScaleCoefficient = 0.85f;
        private const float BounceScaleCoefficient = 0.15f;
        private const float ShakeDirectionTime = 0.15f;
        private const float ShakeScaleTime = 0.5f;
        private const float UpscaleModifier = 1.5f;
        private const float DownscaleModifier = 0.25f;
        private const float ScaleTime = 0.25f;
        private const float FadeDestroy = 0.15f;
        private const string FadeProperty = "_fade";

        [SerializeField] private SpriteRenderer _view;
        [SerializeField] private TMP_Text _numberView;
        [SerializeField] private MoveColorMap _colorMap;

        [Inject] private GridSettings _gridSettings;

        private Transform _transform;
        private Tweener _moveTween;
        private Vector3 _baseScale;
        private Material _material;
        private List<Tweener> _myAnimations = new List<Tweener>();

        public Vector2Int GridPosition { get; private set; }
        public Vector2 WorldPosition => _transform.position;
        public int Number { get; private set; }
        public bool IsWithEffect { get; private set; }
        public bool IsInMove {  get; private set; }

        public event Action<Block> Deactivated;
        public event Action<Block> CameToNewCell;

        private void Awake()
        {
            _transform = transform;
            _baseScale = _transform.localScale;
            _material = _view.material;
        }

        private void OnDisable()
        {
            foreach (var tweener in _myAnimations)
                KillAnimation(tweener);
        }

        public Block Initialize(Transform parent)
        {
            _transform.parent = parent;
            Deactivate();
            return this;
        }

        public void Activate(int number, Vector2Int gridPosition, Color color)
        {
            IsWithEffect = false;
            Number = number;
            AddAnimation(_material.DOFloat(1, FadeProperty, FadeTime));
            _view.enabled = true;
            _view.color = color;
            _numberView.enabled = true;
            _numberView.text = number.ToString();
            _transform.localPosition = (Vector2)gridPosition * _gridSettings.CellSize;
            _transform.localScale = _baseScale;
            GridPosition = gridPosition;
        }

        public void ConnectEffect()
        {
            IsWithEffect = true;
        }

        public void Move(Vector2Int step)
        {
            StopCurrentMoveTween();
            IsInMove = true;
            GridPosition += step;
            _moveTween = AddAnimation(_transform.DOLocalMove((Vector2)GridPosition * _gridSettings.CellSize, _gridSettings.MoveTime).OnComplete(() => CameToNewCell?.Invoke(this)));
            Vector3 scale = _baseScale;

            if (step.y != 0)
                scale.x *= MoveScaleCoefficient;
            else
                scale.y *= MoveScaleCoefficient;

            AddAnimation(_transform.DOScale(scale, _gridSettings.MoveTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localScale = _baseScale));
        }

        public void Merge(Vector3 worldPositionMergedBlock)
        {
            GridPosition = Vector2Int.zero;
            StopCurrentMoveTween();
            Vector3 midpoint = Vector3.Lerp(WorldPosition, worldPositionMergedBlock, 0.5f);
            AddAnimation(_transform.DOMove(midpoint, _gridSettings.MoveTime).OnComplete(Deactivate));
            AddAnimation(_transform.DOShakeScale(_gridSettings.MoveTime, 0.3f, 50, 200));
            AddAnimation(_transform.DOScale(_baseScale * 0.5f, _gridSettings.MoveTime));
            AddAnimation(_material.DOFloat(0, FadeProperty, FadeTime));
        }

        public void Destroy()
        {
            StopCurrentMoveTween();
            Number = 0;
            _numberView.text = "";
            _view.color = _colorMap.Base;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(AddAnimation(_transform.DOScale(_baseScale * DownscaleModifier, ScaleTime)));
            sequence.Append(AddAnimation(_transform.DOScale(_baseScale * UpscaleModifier, ScaleTime))).Join(AddAnimation(_view.DOFade(0f, FadeDestroy))).OnComplete(Deactivate).SetDelay(0.1f);
            sequence.Play();
        }

        public void ChangeNumber(int count)
        {
            Number += count;

            if (Number > 0)
            {
                _numberView.text = Number.ToString();
                _view.color = _colorMap.GetColor(Number);
            }
        }

        public void ShakeDirection(Vector2 direction)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(AddAnimation(_transform.DOLocalMove((Vector2)_transform.localPosition + (direction * BounceScaleCoefficient), ShakeDirectionTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localPosition = (Vector2)GridPosition * _gridSettings.CellSize)));
            float xScale = _transform.localScale.x * (1 + (direction.y == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
            float yScale = _transform.localScale.y * (1 + (direction.x == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
            sequence.Join(AddAnimation(_transform.DOScale(new Vector3(xScale, yScale), ShakeDirectionTime).SetLoops(2, LoopType.Yoyo).OnComplete(() => _transform.localScale = _baseScale)));
            sequence.Play();
        }

        public void ShakeScale()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(AddAnimation(_transform.DOScale(_baseScale * UpscaleModifier, ShakeScaleTime)));
            sequence.Append(AddAnimation(_transform.DOScale(_baseScale, ShakeScaleTime)));
            sequence.Play();
        }

        public void Deactivate()
        {
            Number = 0;
            _view.enabled = false;
            _numberView.enabled = false;
            _transform.localPosition = Vector2.zero;
            _transform.rotation = Quaternion.identity;
            StopCurrentMoveTween();
            Deactivated?.Invoke(this);
        }

        private Tweener AddAnimation(Tweener tweener)
        {
            _myAnimations.Add(tweener);
            return tweener;
        }

        private void StopCurrentMoveTween()
        {
            KillAnimation(_moveTween);
            IsInMove = false;
        }

        private void KillAnimation(Tweener tweener)
        {
            if (tweener != null && tweener.IsActive())
                tweener.Kill();
        }
    }
}
