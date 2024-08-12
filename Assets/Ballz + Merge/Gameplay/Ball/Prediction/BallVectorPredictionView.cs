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
        _ball.EnterGame += HideVew;
    }

    private void OnDisable()
    {
        _predictor.Predicted -= OnChangedPrediction;
        _ball.EnterGame -= HideVew;
    }

    private void OnChangedPrediction(IEnumerable<Vector3> positions)
    {
        var positionsArray = positions.ToArray();
        _lineRenderer.positionCount = positionsArray.Length;
        _lineRenderer.SetPositions(positionsArray);
    }

    private void HideVew() => _lineRenderer.positionCount = 0;
}