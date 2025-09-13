using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueView : MonoBehaviour
{
    [SerializeField] private TMP_Text _value;
    [SerializeField] private Slider _view;
    [SerializeField] private string _label;

    private string _additionalView;

    public void Show(int current, int max)
    {
        gameObject.SetActive(true);
        _view.value = max != 0 ? (float)current / max : _view.maxValue;
        _additionalView = max != 0 ? $" / {max}" : "";
        _value.text = $"{_label}: {current}" + _additionalView;
    }  
}
