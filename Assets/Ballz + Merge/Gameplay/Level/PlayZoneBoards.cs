using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class PlayZoneBoards : CyclicBehavior, ILevelStarter
    {
        [SerializeField] private BoxCollider2D _leftBorder;
        [SerializeField] private BoxCollider2D _rightBorder;
        [SerializeField] private BoxCollider2D _topBorder;
        [SerializeField] private BoxCollider2D _bottomBorder;

        private Dictionary<BoxCollider2D, StartProperty> _collidersProperty;
        private PlayZoneBoards _simulation;
        private bool _isWithSimulation;

        public GameObject GameObject => gameObject;

        private void Awake()
        {
            _collidersProperty = new()
            {
                { _leftBorder, new StartProperty(_leftBorder.offset, _leftBorder.size) },
                { _rightBorder, new StartProperty(_rightBorder.offset, _rightBorder.size) },
                { _topBorder, new StartProperty(_topBorder.offset, _topBorder.size) },
                { _bottomBorder, new StartProperty(_bottomBorder.offset, _bottomBorder.size) }
            };
        }

        public void StartLevel()
        {
            ResetPositionScaleBorder();
        }

        public GameObject GetSimulationClone()
        {
            if (_isWithSimulation == false)
            {
                _simulation = Instantiate(this);
                _isWithSimulation = true;
            }

            return _simulation.gameObject;
        }

        public void ChangePositionScale(bool isVerticalOffset, PositionScaleProperty property)
        {
            if (_isWithSimulation)
                _simulation.ChangePositionScale(isVerticalOffset, property);

            if (isVerticalOffset)
            {
                _rightBorder.offset += property.Scale;
                ChangePositionScaleBorder(_topBorder, property);
                ChangePositionScaleBorder(_bottomBorder, property);
            }
            else
            {
                _topBorder.offset += property.Scale;
                ChangePositionScaleBorder(_leftBorder, property);
                ChangePositionScaleBorder(_rightBorder, property);
            }
        }

        private void ChangePositionScaleBorder(BoxCollider2D border, PositionScaleProperty property)
        {
            border.offset += property.Position;
            border.size += property.Scale;
        }

        private void ResetPositionScaleBorder()
        {
            foreach (var border in _collidersProperty)
            {
                border.Key.offset = border.Value.ColliderOffset;
                border.Key.size = border.Value.ColliderSize;
            }

            if (_isWithSimulation)
                _simulation.ResetPositionScaleBorder();
        }
    }
}