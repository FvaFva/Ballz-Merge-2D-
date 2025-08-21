using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueView : MonoBehaviour
{
    [SerializeField] private TMP_Text _value;
    [SerializeField] private Slider _view;
    [SerializeField] private string _label;

    public void Show(int current, int max)
    {
        gameObject.SetActive(true);
        _view.value = (float)((float)current / (float)(max == 0 ? current : max));
        _value.text = $"{_label}: {current} / {max}";
    }  
}
