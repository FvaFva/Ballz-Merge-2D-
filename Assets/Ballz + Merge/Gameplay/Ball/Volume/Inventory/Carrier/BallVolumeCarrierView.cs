using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeCarrierView : MonoBehaviour
{
    private const float AnimationTime = 0.15f;

    [SerializeField] private AnimatedButton _button;
    [SerializeField] private RectPumper _pumper;
    [SerializeField] private TMP_Text _header;
    [SerializeField] private Image _headerParent;
    [SerializeField] private Image _lock;
    [SerializeField] private BallVolumeView _volumeView;
    [SerializeField] private Button _trigger;

    private float _lockBaseFade;
    private Tween _lockAnimation;

    private void Awake()
    {
        _lockBaseFade = _lock.color.a;
    }

    private void OnEnable() 
    {
        ChangeActive(false);
    }
    
    public void ChangeActive(bool state, IBallVolumeViewData volume = default, string header = "")
    {
        _lockAnimation.Delete();
        _pumper.enabled = state;
        _button.enabled = state;
        _trigger.enabled = state;
        _header.text = header;
        _headerParent.gameObject.SetActive(state);
        _volumeView.Show(volume);

        float targetFade = state ? 0 : _lockBaseFade;
        float targetScale = state ? 2 : 1;
        _lockAnimation = DOTween.Sequence()
            .Join(_lock.DOFade(targetFade, AnimationTime))
            .Join(_lock.transform.DOScale(targetScale, AnimationTime).SetEase(Ease.InOutBounce));
    }
}
