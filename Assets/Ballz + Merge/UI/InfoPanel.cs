using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _value;
    [SerializeField] private TMP_Text _label;

    public InfoPanel Init(float value, string label)
    {
        Show(value);
        _label.text = label;
        return this;
    }

    public void Show(float value)
    {
        Show(value.ToString());
    }

    public void Show(string value)
    {
        _value.text = value;
    }
}
