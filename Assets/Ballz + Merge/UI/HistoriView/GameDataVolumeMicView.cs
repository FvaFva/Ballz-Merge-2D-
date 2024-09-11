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

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(string volumeName, float chance)
    {
        gameObject.SetActive(true);
        BallVolume data = _map.GetVolume(volumeName);
        _chance.text = _map.GetTypifiedChance(data, chance);
        _header.text = data.Name;
        _icon.sprite = data.Icon;
    }
}
