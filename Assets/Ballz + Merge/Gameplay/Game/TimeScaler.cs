using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaler : MonoBehaviour
{
    private const float NormalScale = 1;
    private const float BoostScale = 5;

    [SerializeField] private Button _scaleButton;
    [SerializeField] private TMP_Text _scaleText;

    private bool _isBoosted;

    private void OnEnable()
    {
        _scaleButton.AddListener(OnChangedScale);
    }

    private void OnDisable()
    {
        _scaleButton.RemoveListener(OnChangedScale);
    }

    private void OnChangedScale()
    {
        _isBoosted = !_isBoosted;
        _scaleText.text = $"x{Time.timeScale}";
        Time.timeScale = _isBoosted ? BoostScale: NormalScale;
    }
}