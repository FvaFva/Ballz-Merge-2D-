using DG.Tweening;
using UnityEngine;

public class BallView: BallComponent
{
    private const float TimeHitShow = 0.05f;
    private const float HitScaleCoefficientMin = 0.7f;
    private const float HitScaleCoefficientMax = 1.3f;

    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private ParticleSystem _trailMain;
    [SerializeField] private Color _hitColor;

    private Transform _transform;
    private Vector3 _baseScale;
    private Color _baseColor;

    private void Awake()
    {
        _transform = transform;
        _baseScale = _view.transform.localScale;
        _baseColor = _view.color;
    }

    public void ShowHit(Vector2 hitPosition)
    {
        Vector2Int hiDirection = ((Vector2)_transform.position).CalculateDirection(hitPosition);
        Vector3 scale = _baseScale;

        if (hiDirection.x != 0)
        {
            scale.x *= HitScaleCoefficientMin;
            scale.y *= HitScaleCoefficientMax;
        }
        else
        {
            scale.x *= HitScaleCoefficientMax;
            scale.y *= HitScaleCoefficientMin;
        }
         
        _view.DOColor(_hitColor, TimeHitShow).SetLoops(2, LoopType.Yoyo);
        _view.transform.DOScale(scale, TimeHitShow).SetLoops(2, LoopType.Yoyo).OnComplete(NormalizeState);
    }

    public override void ChangeActivity(bool state)
    {
        _view.enabled = state;
        _trailMain.gameObject.SetActive(state);
    }

    private void NormalizeState()
    {
        _view.transform.localScale = _baseScale;
        _view.color = _baseColor;
    }
}