using System;
using System.Collections.Generic;
using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class PlayZoneBoards : CyclicBehavior, ILevelStarter, IInitializable, IDependentSceneSettings
    {
        [SerializeField] private PlayZoneBoard _leftBorder;
        [SerializeField] private PlayZoneBoard _rightBorder;
        [SerializeField] private PlayZoneBoard _topBorder;
        [SerializeField] private PlayZoneBoard _bottomBorder;

        private List<PlayZoneBoard> _boards;

        private PlayZoneBoards _simulation;
        private bool _isWithSimulation;

        public void Init()
        {
            _boards = new List<PlayZoneBoard>
            {
                _topBorder.Init(),
                _bottomBorder.Init(),
                _leftBorder.Init(),
                _rightBorder.Init()
            };
        }

        public void StartLevel(bool _)
        {
            ResetPositionScaleBorder();
        }

        public GameObject GetSimulationClone()
        {
            if (_isWithSimulation == false)
            {
                _simulation = Instantiate(this);
                _isWithSimulation = true;
                _simulation.transform.position = transform.position;
                _simulation.MarkAsVirtual();
            }

            return _simulation.gameObject;
        }

        public void ApplySetting(SceneSetting sceneSetting)
        {
            bool isDynamicBoards = Convert.ToBoolean(sceneSetting.GetValue(SceneSetting.DynamicBoards));

            foreach (var board in _boards)
                board.ChangeView(isDynamicBoards);
        }

        public void ChangePositionScale(bool isVerticalOffset, PositionScaleProperty property)
        {
            if (_isWithSimulation)
                _simulation.ChangePositionScale(isVerticalOffset, property);

            if (isVerticalOffset)
            {
                _rightBorder.Move(property.Scale);
                _topBorder.AddProperty(property);
                _bottomBorder.AddProperty(property);
            }
            else
            {
                _topBorder.Move(property.Scale);
                _leftBorder.AddProperty(property);
                _rightBorder.AddProperty(property);
            }
        }

        private void MarkAsVirtual()
        {
            Init();

            foreach (var board in _boards)
                board.MarkAsVirtual();
        }

        private void ResetPositionScaleBorder()
        {
            foreach (var board in _boards)
                board.SetBase();

            if (_isWithSimulation)
                _simulation.ResetPositionScaleBorder();
        }
    }
}