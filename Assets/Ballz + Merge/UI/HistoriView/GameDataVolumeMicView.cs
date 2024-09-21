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

    public void Show(string volumeName, float chance)=> ShowData(_map.GetVolume(volumeName), chance);

    public void Show(BallVolumesTypes volumeName, float chance) => ShowData(_map.GetVolume(volumeName), chance);

    private void ShowData(BallVolume data, float chance)
    {
        gameObject.SetActive(true);
        _chance.text = _map.GetTypifiedChance(data, chance);
        _header.text = data.Name;
        _icon.sprite = data.Icon;
    }
}
