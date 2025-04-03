using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _secondsToCycle;

    private void OnEnable ()
    {
        transform.DORotate(new Vector3(0, 0, 360), _secondsToCycle, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        DOTween.Kill(transform);
    }
}
