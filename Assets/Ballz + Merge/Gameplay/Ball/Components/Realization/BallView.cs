using DG.Tweening;
using UnityEngine;

public class BallView: BallComponent
{
    private const float TimeHitShow = 0.05f;
    private const float HitScaleCoefficientMin = 0.7f;
    private const float HitScaleCoefficientMax = 1.3f;

    //[SerializeField] private SpriteRenderer _view;
    [SerializeField] private GameObject _3dView;
    [SerializeField] private ParticleSystem _trailMain;

    private Transform _transform;
    private Vector3 _baseScale;
    private Color _baseColor;

    private void Awake()
    {
        _transform = transform;
        _baseScale = _3dView.transform.localScale;
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
         
        //_view.transform.DOScale(scale, TimeHitShow).SetLoops(2, LoopType.Yoyo).OnComplete(NormalizeState);
        _3dView.transform.DOScale(scale, TimeHitShow).SetLoops(2, LoopType.Yoyo).OnComplete(NormalizeState);
    }

    public override void ChangeActivity(bool state)
    {
        //_view.enabled = state;
        _3dView.SetActive(state);
        _trailMain.gameObject.SetActive(state);
    }

    private void NormalizeState()
    {
        //_view.transform.localScale = _baseScale;
        //_view.color = _baseColor;
        _3dView.transform.localScale = _baseScale;
    }
}