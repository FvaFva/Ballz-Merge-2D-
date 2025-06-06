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
        private const int OutBoardPosition = int.MaxValue;

        [SerializeField] private BlockViewModel _view;
        [SerializeField] private BlockPhysicModel _physic;

        [Inject] private GridSettings _gridSettings;
        [Inject] private BlocksMover _mover;

        private Transform _transform;
        private Tweener _moveTween;
        private TweenCallback _currentAction;
        public bool IsAlive { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public Vector2 WorldPosition => _transform.position;
        public int ID { get; private set; }
        public int Number { get; private set; }
        public bool IsWithEffect { get; private set; }

        public event Action<Block, Vector2Int> Hit;
        public event Action<Block> CameToNewCell;
        public event Action<Block> Deactivated;
        public event Action<Block> Destroyed;
        public event Action<Block, int> NumberChanged;

        public List<string> Debug = new List<string>();

        private void Awake()
        {
            _transform = transform;
            ID = 0;
        }

        private void OnEnable()
        {
            _physic.Hit += OnHit;
        }

        private void OnDisable()
        {
            _physic.Hit -= OnHit;
        }

        public void ChangeID(int id)
        {
            ID = id;
            name = $"Block {ID}";
        }

        public Block Initialize(int id, Transform parent, GridVirtualCell virtualBox)
        {
            Debug.Add("init");
            ChangeID(id);
            _transform.parent = parent;
            _view.Init(_gridSettings.MoveTime, _gridSettings.CellSize);
            _physic.Init(virtualBox);
            IsAlive = true;
            Deactivate();
            return this;
        }

        public void Activate(int id, int number, Vector2Int gridPosition, Color color)
        {
            Debug.Add("Activate");
            ChangeID(id);
            _transform.localPosition = (Vector2)gridPosition * _gridSettings.CellSize;
            IsWithEffect = false;
            IsAlive = true;
            Number = number;
            GridPosition = gridPosition;
            _view.Activate(number, color);
            _physic.Activate();
        }

        public void ConnectEffect()
        {
            Debug.Add("Add effect");
            IsWithEffect = true;
        }

        public bool CanMove(Vector2Int step, bool isMoveDown = false)
        {
            if (IsAlive == false)
                return false;

            if (CantMove(step, isMoveDown))
            {
                PlayBounceAnimation(step);
                return false;
            }

            return true;
        }

        public void Move(Vector2Int step)
        {
            if (IsAlive == false)
                return;

            Debug.Add($"Move {step}");
            StopCurrentMoveTween();
            GridPosition += step;
            var newPosition = (Vector2)GridPosition * _gridSettings.CellSize;
            Tweener tweener = _transform.DOLocalMove(newPosition, _gridSettings.MoveTime).SetAutoKill(true).Pause();
            PlayTween(tweener, () => ExecuteAction(OnComeToNewCell));

            _view.AnimationMove(step);
            _physic.Deactivate();
        }

        public void Merge(Vector3 worldPositionMergedBlock)
        {
            if (IsAlive == false)
                return;

            Debug.Add($"Merge {worldPositionMergedBlock}");
            StopCurrentMoveTween();
            Vector3 midpoint = Vector3.Lerp(WorldPosition, worldPositionMergedBlock, 0.5f);
            Tweener tweener = _transform.DOMove(midpoint, _gridSettings.MoveTime).SetAutoKill(true).Pause();
            PlayTween(tweener, () => ExecuteAction(Deactivate));
            _view.PlayMerge();
            _physic.Deactivate();
        }

        public void Destroy()
        {
            Debug.Add($"Destroy");
            Destroyed?.Invoke(this);
            StopCurrentMoveTween();
            _view.PlayDestroy(Deactivate);
            _physic.Deactivate();
        }

        public void ChangeNumber(int count)
        {
            Debug.Add($"ChangeNumber {count}");
            Number += count;
            _view.ChangeNumber(Number);

            if (Number <= 0)
                Destroy();
            else
                NumberChanged?.Invoke(this, count);
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
            if (IsAlive == false)
                return;

            Debug.Add($"Deactivate");
            Deactivated?.Invoke(this);
            IsAlive = false;
            StopCurrentMoveTween();
            _view.Deactivate();
            _physic.Deactivate();
            Number = 0;
            GridPosition = Vector2Int.zero;
            _transform.localPosition = Vector2.one * OutBoardPosition;
            _transform.rotation = Quaternion.identity;
        }

        private void ExecuteAction(Action action)
        {
            _currentAction = null;
            action.Invoke();
        }

        private void PlayTween(Tweener tweener, TweenCallback action)
        {
            _currentAction?.Invoke();

            _currentAction = action;
            _moveTween = tweener.OnComplete(_currentAction).Play();
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

            if (IsAlive)
                _physic.Activate();
        }

        private void OnHit(Vector2Int direction) => Hit?.Invoke(this, direction);

        private bool CantMove(Vector2Int step, bool isMoveDown)
        {
            bool downVector = step == Vector2Int.down;
            bool forceDownMove = isMoveDown && downVector;

            if (forceDownMove)
                return false;

            if (downVector)
                return true;

            return _mover.IsFree(GridPosition + step) == false;
        }
    }
}
