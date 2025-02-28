using BallzMerge.Gameplay.Level;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace BallzMerge.Gameplay.BlockSpace
{
    public class Block : MonoBehaviour
    {
        private static int _id = 0;
        static public int ID { get { return ++_id; } }
        [SerializeField] private BlockViewModel _view;
        [SerializeField] private BlockPhysicModel _physic;

        [Inject] private GridSettings _gridSettings;

        private Transform _transform;
        private Tweener _moveTween;
        private bool _isAlive;

        public Vector2Int GridPosition { get; private set; }
        public Vector2 WorldPosition => _transform.position;
        public int Number { get; private set; }
        public bool IsWithEffect { get; private set; }

        public event Action<Block, Vector2Int> Hit;
        public event Action<Block> CameToNewCell;
        public event Action<Block> Freed;
        public event Action<Block> Destroyed;
        public event Action<Block> NumberChanged;

        public List<string> Debug = new List<string>();

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            _physic.Hit += OnHit;
        }

        private void OnDisable()
        {
            _physic.Hit -= OnHit;
        }

        public Block Initialize(Transform parent, GridVirtualCell virtualBox)
        {
            Debug.Add("init");
            name = $"Block {ID}";
            _transform.parent = parent;
            _view.Init(_gridSettings.MoveTime, _gridSettings.CellSize);
            _physic.Init(virtualBox);
            _isAlive= true;
            Deactivate();
            return this;
        }

        public void Activate(int number, Vector2Int gridPosition, Color color)
        {
            Debug.Add("Activate");
            _transform.localPosition = (Vector2)gridPosition * _gridSettings.CellSize;
            IsWithEffect = false;
            Number = number;
            GridPosition = gridPosition;
            _view.Activate(number, color);
            _physic.Activate();
        }

        public void ConnectEffect()
        {
            IsWithEffect = true;
        }

        public void Move(Vector2Int step)
        {
            Debug.Add($"Move {step}");
            StopCurrentMoveTween();
            GridPosition += step;
            var newPosition = (Vector2)GridPosition * _gridSettings.CellSize;
            _moveTween = _transform
                .DOLocalMove(newPosition, _gridSettings.MoveTime)
                .OnComplete(OnComeToNewCell);
            
            _view.AnimationMove(step);
            _physic.Deactivate();
        }

        public void Merge(Vector3 worldPositionMergedBlock)
        {
            _isAlive = false;
            Debug.Add($"Merge {worldPositionMergedBlock}");
            GridPosition = Vector2Int.zero;
            StopCurrentMoveTween();
            Vector3 midpoint = Vector3.Lerp(WorldPosition, worldPositionMergedBlock, 0.5f);
            _transform.DOMove(midpoint, _gridSettings.MoveTime).OnComplete(Deactivate);
            _view.PlayMerge();
            _physic.Deactivate();
        }

        public void Destroy()
        {
            _isAlive = false;
            Debug.Add($"Destroy");
            Destroyed?.Invoke(this);
            StopCurrentMoveTween();
            Number = 0;
            _view.PlayDestroy(Deactivate);
            _physic.Deactivate();
        }

        public void ChangeNumber(int count)
        {
            Debug.Add($"ChangeNumber {count}");
            Number += count;
            _view.ChangeNumber(Number);

            if(Number == 0)
                Destroy();
            else
                NumberChanged?.Invoke(this);
        }

        public void PlayBounceAnimation(Vector2 direction)
        {
            _view.PlayBounce(direction, GridPosition);
        }

        public void PlayShakeAnimation()
        {
            _view.PlayShake();
        }

        public void Deactivate()
        {
            Debug.Add($"Deactivate");
            DOTween.Kill(_transform);
            _view.Deactivate();
            _physic.Deactivate();
            Number = 0;
            _transform.localPosition = Vector2.zero;
            _transform.rotation = Quaternion.identity;
            StopCurrentMoveTween();
            Freed?.Invoke(this);
        }

        private void StopCurrentMoveTween()
        {
            if (_moveTween != null && _moveTween.IsActive())
                _moveTween.Kill();
        }

        private void OnComeToNewCell()
        {
            Debug.Add($"OnComeToNewCell");
            CameToNewCell?.Invoke(this);

            if(_isAlive)
                _physic.Activate();
        }

        private void OnHit(Vector2Int direction) => Hit?.Invoke(this, direction);
    }
}
