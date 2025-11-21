using UnityEngine;
using System.Collections.Generic;

public class PlayerScoresView : DependentColorUI, IInitializable, ILevelStarter, ILevelFinisher
{
    [SerializeField] private List<ValueViewProperty> _valueViewProperties;
    [SerializeField] private BackgroundUI _backgroundUI;

    private Dictionary<IValueViewScore, ValueViewProperty> ValueViews;

    private void OnValidate()
    {
        PlatformRunner.Run(null,
            editorAction: () =>
            {
                foreach (var property in _valueViewProperties)
                    property.Init();
            });
    }

    public void Init()
    {
        ValueViews = new Dictionary<IValueViewScore, ValueViewProperty>();

        foreach (var property in _valueViewProperties)
        {
            property.Init();
            ValueViews.Add(property.Counter, property);
            property.Load();
        }
    }

    public override void ApplyColors(GameColors gameColors)
    {
        _backgroundUI.ApplyColors(gameColors);

        foreach(var valueView in _valueViewProperties)
            valueView.ApplyColors(gameColors);
    }

    public void StartLevel(bool isAfterLoad = false)
    {
        foreach (var property in ValueViews)
            property.Value.Counter.ScoreChanged += UpdateScore;
    }

    public void FinishLevel()
    {
        foreach (var property in ValueViews)
            property.Value.Counter.ScoreChanged -= UpdateScore;
    }

    private void UpdateScore(IValueViewScore viewScore, int current, int total)
    {
        ValueViews[viewScore].ValueView.Show(current, total);
    }
}
