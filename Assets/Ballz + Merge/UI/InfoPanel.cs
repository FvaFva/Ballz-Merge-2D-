using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _value;
    [SerializeField] private TMP_Text _label;

    public InfoPanel Init(float value, string label)
    {
        Show(value);
        Hide();
        _label.text = label;
        return this;
    }

    public void Show(float value)
    {
        Show(value.ToString("F0"));
    }

    public void Show(string value)
    {
        gameObject.SetActive(true);
        _value.text = value;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }    
}
