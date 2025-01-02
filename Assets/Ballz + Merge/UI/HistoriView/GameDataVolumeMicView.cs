using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameDataVolumeMicView : MonoBehaviour
{
    [SerializeField] private TMP_Text _header;
    [SerializeField] private TMP_Text _chance;
    [SerializeField] private Image _icon;

    [Inject] private BallVolumesMap _map;

    public GameDataVolumeMicView Init()
    {
        ProjectContext.Instance.Container.Inject(this);
        return this;
    }

    public GameDataVolumeMicView Hide()
    {
        gameObject.SetActive(false);
        return this;
    }

    public void Show(string volumeName, int value)=> Show(_map.GetVolume(volumeName), value);

    public void Show(BallVolumesTypes volumeType, int value) => Show(_map.GetVolume(volumeType), value);

    public void Show(BallVolume volume, int value)
    {
        gameObject.SetActive(true);
        _chance.text = value.ToString();
        _header.text = volume.Name;
        _icon.sprite = volume.Icon;
    }
}
