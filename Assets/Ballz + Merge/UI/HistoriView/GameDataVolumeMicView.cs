using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameDataVolumeMicView : MonoBehaviour
{
    [SerializeField] private TMP_Text _header;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private Image _icon;
    [SerializeField] private VolumeMicAdditionalView _additional;

    [Inject] private BallVolumesMap _map;

    private Sprite _sprite;

    public BallVolumesBagCell Data {  get; private set; }
    public bool IsActive { get; private set; }

    public event Action<GameDataVolumeMicView> Performed;

    private void OnEnable()
    {
        _additional.Performed += OnPerform;
    }

    private void OnDisable()
    {
        _additional.Performed -= OnPerform;
        IsActive = false;
    }

    public GameDataVolumeMicView Init()
    {
        ProjectContext.Instance.Container.Inject(this);
        _sprite = _icon.sprite;
        return this;
    }

    public GameDataVolumeMicView Hide()
    {
        gameObject.SetActive(false);
        IsActive = false;
        _additional.Unperformed();
        return this;
    }

    public void Unperformed()
    {
        _additional.Unperformed();
    }

    public void Clear()
    {
        _level.text = "";
        _header.text = "";
        _additional.SetBaseView();
        IsActive = false;
        _icon.sprite = _sprite;
    }

    public void Show(string volumeName, int level)
    {
        var volume = _map.GetVolume(volumeName);
        var rarity = _map.GetRarity(level);

        if (rarity == null)
            Show(volume, level, volume.GetDescription(level));
        else
            Show(new BallVolumesBagCell(volume, rarity));
    }

    public void Show(BallVolumesBagCell data)
    {
        Data = data;
        var description = Data.Volume.GetDescription(Data.Rarity);
        Show(Data.Volume, Data.Rarity.Weight, description);
    }

    private void Show(BallVolume volume, int level, string description)
    {
        gameObject.SetActive(true);

        if (volume == null)
        {
            Clear();
            return;
        }

        _additional.Show(description);
        _level.text = level.ToString();
        _header.text = volume?.Name;
        _icon.sprite = volume?.Icon;
    }

    private void OnPerform(bool isActive)
    {
        IsActive = isActive;
        Performed?.Invoke(this);
    }
}
