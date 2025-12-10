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
        private Vector2 _newPosition;
        private Tweener _moveTween;
        private TweenCallback _onCompleteTweenAction;
        private Dictionary<BlockMoveActionType, Action> _blockMoveTypeActions;

        public bool IsInMerge { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsInMove { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public Vector2 WorldPosition => _transform.position;
        public int ID { get; private set; }
        public int Number { get; private set; }
        public bool IsWithEffect { get; private set; }

        public event Action<Block, Vector2Int> Hit;
        public event Action<Block> ChangedPosition;
        public event Action<Block> Moved;
        public event Action<Block> Deactivated;
        public event Action<Block> Destroyed;
        public event Action<Block, int> NumberChanged;

        public List<string> Debug = new List<string>();

        private void Awake()
        {
            _transform = transform;
            ID = 0;
            _blockMoveTypeActions = new Dictionary<BlockMoveActionType, Action>()
            {
                { BlockMoveActionType.ChangePosition, () => ChangedPosition?.Invoke(this) },
                { BlockMoveActionType.Move, () => Moved?.Invoke(this) }
            };
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

        public Block Initialize(int id, Transform parent, GridVirtualCell virtualBox, BlocksSettings settings)
        {
            Debug.Add("init");
            ChangeID(id);
            _transform.parent = parent;
            _view.Init(_gridSettings.MoveTime, settings);
            _physic.Init(virtualBox);
            IsAlive = true;
            IsInMerge = false;
            IsInMove = false;
            Deactivate();
            return this;
        }

        public void Activate(int id, int number, Vector2Int gridPosition, Color color)
        {
            Debug.Add("Activate");
            ChangeID(id);
            _transform.localPosition = (Vector2)gridPosition * _gridSettings.CellSize;
            _newPosition = _transform.localPosition;
            IsWithEffect = false;
            IsAlive = true;
            IsInMerge = false;
            IsInMove = false;
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

        public bool CanMove(Vector2Int step, bool isHit = true)
        {
            if (IsAlive == false)
                return false;

            if (_mover.IsFree(GridPosition + step) == false)
            {
                if (isHit)
                    PlayBounceAnimation(step);

                return false;
            }

            if (step == Vector2Int.down && isHit)
            {
                PlayBounceAnimation(step);
                return false;
            }

            return true;
        }

        public void MoveTo(Vector2Int position, BlockMoveActionType moveType)
        {
            if (IsAlive == false)
                return;

            Debug.Add($"Move to {position}");
            StopCurrentMoveTween();
            GridPosition = position;
            _newPosition = (Vector2)GridPosition * _gridSettings.CellSize;
            _transform.localPosition = _newPosition;
            _blockMoveTypeActions[moveType]?.Invoke();
        }

        public void Move(Vector2Int step, BlockMoveActionType moveType)
        {
            if (IsAlive == false || IsInMerge == true || IsInMove == true)
                return;

            IsInMove = true;
            Debug.Add($"Move {step}");
            StopCurrentMoveTween();
            GridPosition += step;
            _newPosition = (Vector2)GridPosition * _gridSettings.CellSize;
            Tweener tweener = _transform.DOLocalMove(_newPosition, _gridSettings.MoveTime).Pause();
            PlayTween(tweener, () => OnComeToNewCell(moveType, _blockMoveTypeActions[moveType]));

            _view.AnimationMove(step);
            _physic.Deactivate();
        }

        public void Merge(Block mergedBlock)
        {
            if (IsAlive == false || IsInMerge == true)
                return;

            Debug.Add($"Merge with {mergedBlock.name}");
            IsInMerge = true;
            StopCurrentMoveTween();
            _newPosition = Vector2.Lerp(WorldPosition, mergedBlock.WorldPosition, 0.5f);
            Tweener tweener = _transform.DOMove(_newPosition, _gridSettings.MoveTime).Pause();
            PlayTween(tweener, Deactivate);
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

        public void PlayBounceAnimation(Vector2Int direction)
        {
            _view.PlayBounce(direction);
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
            IsAlive = false;
            _transform.localPosition = Vector2.one * OutBoardPosition;
            _transform.rotation = Quaternion.identity;
            _newPosition = _transform.localPosition;
            StopCurrentMoveTween();
            Deactivated?.Invoke(this);
            _view.Deactivate();
            _physic.Deactivate();
            Number = 0;
            GridPosition = Vector2Int.zero;
        }

        private void PlayTween(Tweener tweener, TweenCallback action)
        {
            if (_onCompleteTweenAction != action)
            {
                _onCompleteTweenAction = action;
                _moveTween = tweener.OnComplete(ExecuteAction).Play();
            }
        }

        private void ExecuteAction()
        {
            TweenCallback callback = _onCompleteTweenAction;
            _onCompleteTweenAction = null;
            _moveTween = null;
            callback?.Invoke();
        }

        private void StopCurrentMoveTween()
        {
            if (_moveTween != null && _moveTween.IsActive())
            {
                _moveTween.Kill();
                ExecuteAction();
                _transform.localPosition = _newPosition;
            }
        }

        private void OnComeToNewCell(BlockMoveActionType moveType, Action action)
        {
            Debug.Add($"OnComeToNewCell: {moveType}");
            action?.Invoke();
            IsInMove = false;

            if (IsAlive)
                _physic.Activate();
        }

        private void OnHit(Vector2Int direction)
        {
            Hit?.Invoke(this, direction);
            Debug.Add("OnBlockHit");
        }
    }
}
