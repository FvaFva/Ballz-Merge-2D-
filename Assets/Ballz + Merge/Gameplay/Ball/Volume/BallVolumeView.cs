using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _additionalInfor;
    [SerializeField] private TMP_Text _level;

    public void Show(BallVolumesBagCell data)
    {
        gameObject.SetActive(true);
        _icon.sprite = data.Volume.Icon;
        _name.text = data.Volume.Name;
        _description.text = data.Description;
        _additionalInfor.text = data.Suffix;
        _level.text = data.Value.ToString();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
