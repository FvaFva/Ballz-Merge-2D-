using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _waves;
    [SerializeField] private TMP_Text _count;
    [SerializeField] private Button _trigger;
    [SerializeField] private AnimatedButton _triggerView;
    [SerializeField] private LevelVolumesView _volumesPrefab;
    [SerializeField] private RectTransform _volumesContent;
    [SerializeField] private LevelBlockView _blockPrefab;
    [SerializeField] private RectTransform _blockContent;

    private LevelSettings _current;
    private List<LevelVolumesView> _volumes = new List<LevelVolumesView>();
    private List<LevelBlockView> _blocks = new List<LevelBlockView>();

    public event Action<LevelSettings> Chose;

    private void OnEnable()
    {
        _trigger.AddListener(OnTriggered);
        ChangeSelectorActivity(true);
    }

    private void OnDisable()
    {
        _trigger.RemoveListener(OnTriggered);
        HideViews();
    }

    public void Show(LevelSettings level)
    {
        _current = level;
        _title.text = level.Title;
        _waves.text = level.BlocksSettings.SpawnProperties.Count.ToString();
        int minCount = level.BlocksSettings.SpawnProperties.Sum(wave => wave.Count.Min(p => p.Value));
        int maxCount = level.BlocksSettings.SpawnProperties.Sum(wave => wave.Count.Max(p => p.Value));

        _count.text = minCount == maxCount ? minCount.ToString() : $"{minCount} - {maxCount}";

        HideViews();
        ChangeSelectorActivity(true);

        foreach (var map in level.DropSettings.DropMap)
        {
            var view = GetView(_volumes, () => Instantiate(_volumesPrefab, _volumesContent));
            var names = map.Value.Select(v => v.Name);
            view.Show(map.Key, names);
        }

        var allNumbers = level.BlocksSettings.SpawnProperties.SelectMany(s => s.Number).Select(n => n.Value).Distinct().OrderBy(n => n);

        foreach (var number in allNumbers)
            GetView(_blocks, () => Instantiate(_blockPrefab, _blockContent)).Show(level.BlocksSettings.GetColor(number), number);
    }

    private void OnTriggered() => Chose?.Invoke(_current);

    private void HideViews()
    {
        foreach (var block in _blocks)
            block.Hide();

        foreach (var volume in _volumes)
            volume.Hide();
    }

    private void ChangeSelectorActivity(bool state)
    {
        _trigger.interactable = state;
        _triggerView.SetState(state);
    }

    private T GetView<T>(List<T> all, Func<T> generate) where T : IAvailable
    {
        T view = all.Where(v => v.IsAvailable).FirstOrDefault();

        if (view == null)
        {
            view = generate();
            all.Add(view);
        }

        return view;
    }
}
