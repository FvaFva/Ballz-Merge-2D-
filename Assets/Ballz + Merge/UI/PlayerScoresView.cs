using UnityEngine;
using System.Collections.Generic;

public class PlayerScoresView : CyclicBehavior, IInitializable, ILevelStarter, ILevelFinisher
{
    [SerializeField] private List<ValueViewProperty> _valueViewProperties;

    private Dictionary<IValueViewScore, ValueViewProperty> ValueViews;

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var property in _valueViewProperties)
            property.Init();
    }
#endif

    public void Init()
    {
        ValueViews = new Dictionary<IValueViewScore, ValueViewProperty>();

        foreach (var property in _valueViewProperties)
        {
            property.Init();
            ValueViews.Add(property.Counter, property);
        }
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

        ValueViews.Clear();
    }

    private void UpdateScore(IValueViewScore viewScore, int current, int total)
    {
        ValueViews[viewScore].ValueView.Show(current, total);
    }
}
