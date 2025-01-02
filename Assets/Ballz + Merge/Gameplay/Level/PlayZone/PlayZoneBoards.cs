using UnityEngine;

namespace BallzMerge.Gameplay.Level
{
    public class PlayZoneBoards : CyclicBehavior, ILevelStarter, IInitializable
    {
        [SerializeField] private PlayZoneBoard _leftBorder;
        [SerializeField] private PlayZoneBoard _rightBorder;
        [SerializeField] private PlayZoneBoard _topBorder;
        [SerializeField] private PlayZoneBoard _bottomBorder;
        [SerializeField] private PlayZoneBoard _bottomVisual;
        [SerializeField] private PlayZoneBoard _strikeField;

        private PlayZoneBoards _simulation;
        private bool _isWithSimulation;

        public void Init()
        {
            _topBorder.Init();
            _bottomBorder.Init();
            _leftBorder.Init();
            _rightBorder.Init();
            _bottomVisual.Init();
            _strikeField.Init();
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
                _simulation.MarkAsVirtual();
            }

            return _simulation.gameObject;
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

                if (_bottomVisual != null && _strikeField != null)
                {
                    _bottomVisual.AddProperty(property);
                    _strikeField.AddProperty(property);
                }
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
            _topBorder.MarkAsVirtual();
            _bottomBorder.MarkAsVirtual();
            _leftBorder.MarkAsVirtual();
            _rightBorder.MarkAsVirtual();
            _bottomVisual = null;
            _strikeField = null;
        }

        private void ResetPositionScaleBorder()
        {
            _topBorder.SetBase();
            _bottomBorder.SetBase();
            _leftBorder.SetBase();
            _rightBorder.SetBase();

            if (_isWithSimulation)
                _simulation.ResetPositionScaleBorder();
        }
    }
}