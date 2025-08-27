using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameDataVolumeMicView : MonoBehaviour, IAvailable
{
    [SerializeField] private TMP_Text _header;
    [SerializeField] private List<Image> _rarityImages;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private Image _icon;
    [SerializeField] private VolumeMicAdditionalView _additional;

    [Inject] private BallVolumesMap _map;

    private Sprite _sprite;

    public IBallVolumesBagCell<BallVolume> Data { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsAvailable { get; private set; }

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
        IsAvailable = true;
        return this;
    }

    public GameDataVolumeMicView Hide()
    {
        gameObject.SetActive(false);
        IsActive = false;
        IsAvailable = true;
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
        IsAvailable = true;
        _icon.sprite = _sprite;
    }

    public void Show(string volumeName, int level, int? count = null)
    {
        var volume = _map.GetVolumeByName(volumeName);
        var rarity = _map.GetRarity(level);

        if (volume == null)
            return;

        if (rarity == null)
            Show(volume, count ?? level, volume.GetDescription(level));
        else
            Show(new BallVolumesBagCell<BallVolume>(volume, rarity), count);
    }

    public void Show(IBallVolumesBagCell<BallVolume> data, int? count = null)
    {
        Data = data;
        Show(Data.Volume, count ?? Data.Value, Data.Description, Data.RarityColor);
    }

    private void Show(BallVolume volume, int level, string description, Color? rarityColor = null)
    {
        gameObject.SetActive(true);
        IsAvailable = false;

        if (volume == null)
        {
            Clear();
            return;
        }

        _additional.Show(description);
        _level.text = level.ToString();

        foreach (Image rarityImage in _rarityImages)
            rarityImage.color = rarityColor ?? rarityImage.color;

        _header.text = volume.Name;
        _icon.sprite = volume.Icon;
    }

    private void OnPerform(bool isActive)
    {
        IsActive = isActive;
        Performed?.Invoke(this);
    }
}
