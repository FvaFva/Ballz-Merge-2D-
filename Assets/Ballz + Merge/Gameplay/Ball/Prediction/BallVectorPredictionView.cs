using BallzMerge.Gameplay.BallSpace;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class BallVectorPredictionView : BallComponent
{
    [SerializeField] private BallVectorPredictor _predictor;
    [SerializeField] private LineRenderer _lineRenderer;

    [Inject] private Ball _ball;

    private void OnEnable()
    {
        _predictor.Predicted += OnChangedPrediction;
        _ball.EnterGame += HideView;
        _ball.LeftGame += ShowView;
    }

    private void OnDisable()
    {
        _predictor.Predicted -= OnChangedPrediction;
        _ball.EnterGame -= HideView;
        _ball.LeftGame -= ShowView;
    }

    private void OnChangedPrediction(IEnumerable<Vector3> positions)
    {
        var positionsArray = positions.ToArray();
        _lineRenderer.positionCount = positionsArray.Length;
        _lineRenderer.SetPositions(positionsArray);
    }

    private void HideView()
    {
        _predictor.Predicted -= OnChangedPrediction;
        _lineRenderer.positionCount = 0;
    }

    private void ShowView()
    {
        _predictor.Predicted += OnChangedPrediction;
    }
}