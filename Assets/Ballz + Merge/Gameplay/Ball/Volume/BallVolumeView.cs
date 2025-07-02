using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallVolumeView : MonoBehaviour
{
    [SerializeField] private Button _trigger;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _level;

    private Sprite _baseIcon;
    private List<(Action<bool>, Action<IBallVolumeViewData>)> _items = new List<(Action<bool>, Action<IBallVolumeViewData>)>();

    public IBallVolumeViewData CurrentData { get; private set; }
    public event Action<BallVolumeView> Triggered;

    private void Awake()
    {
        if (_icon != null)
        {
            _baseIcon = _icon.sprite;
            _items.Add(((state) => { if (state) _icon.sprite = _baseIcon; }, (data) => { _icon.sprite = data.Icon; }));
        }

        if (_name != null)
            _items.Add(((state) => { _name.SetActive(state); }, (data) => { _name.text = data.Name; }));

        if (_level != null)
            _items.Add(((state) => { _level.SetActive(state); }, (data) => { _level.text = _level.text = data.Value.ToString(); }));

        if (_description != null)
            _items.Add(((state) => { _description.SetActive(state); }, (data) => { _description.text = data.Description; }));
    }

    private void OnEnable()
    {
        _trigger?.AddListener(OnTrigger);
    }

    private void OnDisable()
    {
        _trigger?.RemoveListener(OnTrigger);
    }

    public BallVolumeView Deactivate()
    {
        gameObject.SetActive(false);
        return this;
    }

    public BallVolumeView Show(IBallVolumeViewData data)
    {
        bool isHaveData = data != null;
        CurrentData = data;
        gameObject.SetActive(true);

        foreach (var item in _items)
            item.Item1(isHaveData);

        if (isHaveData)
        {
            foreach (var item in _items)
                item.Item2(data);
        }

        return this;
    }

    private void OnTrigger()
    {
        Triggered?.Invoke(this);
    }
}
