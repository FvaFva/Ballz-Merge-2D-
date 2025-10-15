using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueView : MonoBehaviour
{
    [SerializeField] private TMP_Text _value;
    [SerializeField] private Slider _view;
    [SerializeField] private string _label;

    private string _additionalView;
    private ValueChanger _valueChanger;

    public void Init()
    {
        _valueChanger = new ValueChanger();
    }

    public void Show(int current, int max)
    {
        SetValue((float)current / max, max);
        _value.text = $"{_label}: {current}" + _additionalView;
    }

    private void SetValue(float endValue, float maxValue)
    {
        if (maxValue <= 0)
        {
            _view.value = _view.maxValue;
            _additionalView = "";
            return;
        }

        _additionalView = $" / {maxValue}";
        _valueChanger.ChangeValueOverTime(_view.value, endValue, (newValue) => _view.value = newValue, duration : 0.3f);
            
    }
}
