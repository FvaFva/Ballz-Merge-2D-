using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _additionalInformation;
    [SerializeField] private TMP_Text _level;

    private Sprite _baseIcon;

    private void Awake()
    {
        _baseIcon = _icon.sprite;
    }

    public void Show(BallVolumesBagCell data)
    {
        bool isHaveData = data != null;
        ChangeElementsActivity(isHaveData);
        gameObject.SetActive(true);

        if (isHaveData == false)
        {
            _icon.sprite = _baseIcon;
            return;
        }

        _icon.sprite = data.Volume.Icon;
        _name.text = data.Volume.Name;
        _description.text = data.Description;
        _additionalInformation.text = data.Suffix;
        _level.text = data.Value.ToString();
    }

    private void ChangeElementsActivity(bool state)
    {
        _name.SetActive(state);
        _description.SetActive(state);
        _additionalInformation.SetActive(state);
        _level.SetActive(state);
    }
}
