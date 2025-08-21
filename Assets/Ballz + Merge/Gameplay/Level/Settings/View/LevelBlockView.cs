using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBlockView : MonoBehaviour, IAvailable
{
    [SerializeField] private Image _view;
    [SerializeField] private TMP_Text _number;

    public bool IsAvailable { get; private set; }

    public void Hide()
    {
        IsAvailable = true;
        _number.text = "";
        gameObject.SetActive(false);
    }

    public void Show(Color color, int number)
    {
        _view.color = color;
        _number.text = number.ToString();
        IsAvailable = false;
        gameObject.SetActive(true);
    }
}
