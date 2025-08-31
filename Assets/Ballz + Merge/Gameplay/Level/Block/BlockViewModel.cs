using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class BlockViewModel : MonoBehaviour
    {
        private const string FadeProperty = "_fade";
        private const float FadeTime = 0.6f;
        private const float MoveScaleCoefficient = 0.85f;
        private const float DownscaleModifier = 0.25f;
        private const float ScaleTime = 0.25f;
        private const float UpscaleModifier = 1.5f;
        private const float FadeDestroy = 0.15f;
        private const float ShakeScaleTime = 0.5f;
        private const float BounceScaleCoefficient = 0.15f;
        private const float BounceTime = 0.15f;

        [SerializeField] private ParticleSystem _hitEffect;
        [SerializeField] private SpriteRenderer _colorSkin;
        [SerializeField] private SpriteRenderer _stoneSkin;
        [SerializeField] private TMP_Text _number;
        [SerializeField] private List<Sprite> _stoneSkins;

        private BlocksSettings _colorMap;
        private Transform _transform;
        private Vector3 _baseScale;
        private Material _material;
        private float _moveTime;
        private Dictionary<Vector2Int, float> _rotationsToDictionary;

        private void Awake()
        {
            _transform = transform;
            _baseScale = _transform.localScale;
            _material = new Material(_colorSkin.material);
            _colorSkin.material = _material;
            _stoneSkin.material = _material;
            _rotationsToDictionary = new Dictionary<Vector2Int, float>
            {
                { Vector2Int.left, 0f },
                { Vector2Int.down, 90f },
                { Vector2Int.right, 180f },
                { Vector2Int.up, 270f }
            };
        }

        private void OnEnable()
        {
            _transform.localScale = _baseScale;
        }

        private void OnDisable()
        {
            DOTween.Kill(_transform);
            DOTween.Kill(_colorSkin);
            DOTween.Kill(_material);
        }

        public void Init(float moveTime, BlocksSettings settings)
        {
            _moveTime = moveTime;
            _colorMap = settings;
            _material.SetFloat(FadeProperty, 0);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            ChangeNumber(0);
        }

        public void Activate(int number, Color color)
        {
            gameObject.SetActive(true);
            _transform.localPosition = Vector3.zero;
            _transform.localScale = _baseScale;
            _colorSkin.color = color;
            _stoneSkin.sprite = _stoneSkins[Random.Range(0, _stoneSkins.Count)];
            _material.DOFloat(1, FadeProperty, FadeTime);
            _number.text = number.ToString();
        }

        public void PlayDestroy(TweenCallback onDestroyed)
        {
            _number.text = "";
            _colorSkin.color = new Color();
            Sequence sequence = DOTween.Sequence();
            var downScale = _baseScale * DownscaleModifier;
            var upScale = _baseScale * UpscaleModifier;
            sequence.Append(_transform.DOScale(downScale, ScaleTime));
            sequence.Append(_transform.DOScale(upScale, ScaleTime))
                .Join(_colorSkin.DOFade(0f, FadeDestroy))
                .Join(_stoneSkin.DOFade(0f, FadeDestroy))
                .OnComplete(onDestroyed)
                .SetDelay(0.1f);
            sequence.Play();
        }

        public void PlayBounce(Vector2Int direction)
        {
            Sequence sequence = DOTween.Sequence();
            var newPosition = (Vector2)_transform.localPosition + ((Vector2)direction * BounceScaleCoefficient);
            float xScale = _transform.localScale.x * (1 + (direction.y == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
            float yScale = _transform.localScale.y * (1 + (direction.x == 0 ? -1 * BounceScaleCoefficient : BounceScaleCoefficient));
            PlayEffect(direction);

            sequence.Append(_transform
                .DOLocalMove(newPosition, BounceTime)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _transform.localPosition = Vector3.zero));

            sequence.Join(_transform
                .DOScale(new Vector3(xScale, yScale), BounceTime)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _transform.localScale = Vector3.one));

            sequence.Play();
        }

        public void PlayShake()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_transform.DOScale(_baseScale * UpscaleModifier, ShakeScaleTime));
            sequence.Append(_transform.DOScale(_baseScale, ShakeScaleTime));
            sequence.Play();
        }

        public void AnimationMove(Vector2Int step)
        {
            Vector3 scale = _baseScale;

            if (step.y != 0)
                scale.x *= MoveScaleCoefficient;
            else
                scale.y *= MoveScaleCoefficient;

            _transform.DOScale(scale, _moveTime)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _transform.localScale = _baseScale);
        }

        public void PlayMerge()
        {
            _transform.DOShakeScale(_moveTime, 0.3f, 50, 200);
            _transform.DOScale(_baseScale * 0.5f, _moveTime);
            _material.DOFloat(0, FadeProperty, FadeTime);
        }

        public void ChangeNumber(int number)
        {
            _number.text = number.ToString();
            _colorSkin.color = _colorMap.GetColor(number);
        }

        private void PlayEffect(Vector2Int direction)
        {
            _hitEffect.transform.rotation = Quaternion.Euler(0, 0, _rotationsToDictionary[direction]);
            _hitEffect.Play();
        }
    }
}
